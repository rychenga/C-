/*
 * Object.cpp
 *	讀取 Wavefront Obj 檔案，並顯示繪在螢幕上
 *  第8章 新增材質影像
 *
 */

#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include "object.h"
#include "bitmap.h"

#define T(x) (model->triangles[(x)])

/*------ Group 以串列的方式組成 ------*/
// 找尋名稱為 name 的 Group
ObjGroup* FindGroup(ObjModel* model, char* name)
{
	ObjGroup* group;
	assert(model);

	group = model->groups;		
	while(group)
	{
		if(!strcmp(name, group->name))		// 找到相同名稱則離開
			break;

		group = group->next;
	}
	return group;
}

/*------ Material 找尋 ------*/
GLuint FindMaterial(ObjModel* model, char* name)
{
	GLuint i;

	for(i=0; i < model->nummaterials; i++)
	{
		if(!strcmp(model->materials[i].name, name))
			goto found;
	}
	printf("This file %s can't find\n", name);
	i=0;

found:
	return i;
}

/*------第8章新增 取得Texture訊息 ------*/
static void LoadBMPTexture(ObjMaterial* material)
{
	BITMAPINFO	texInfo;				// BMP header
	material->bitmap = LoadBitmapFile(material->map_name, &texInfo);
	assert(material->bitmap);

	material->width  = texInfo.bmiHeader.biWidth;
	material->height = texInfo.bmiHeader.biHeight;
}

// 加入名稱為 name 的 Group
ObjGroup* AddGroup(ObjModel* model, char* name)
{
	ObjGroup* group;
	group = FindGroup(model, name);

	// 假如沒有找到才新增
	if(!group)
	{
		group = (ObjGroup*)malloc(sizeof(ObjGroup));
		group->name = strdup(name);
		group->material = 0;
		group->numtriangles = 0;
		group->next = model->groups;
		model->groups = group;	
		model->numgroups++;
	}
	return group;
}

//
//	讀取 MTL 檔案內的資料存入 
//
void ReadMTL(ObjModel* model, char* name)
{
	FILE* fp;
	char buf[128];
	GLuint nummaterials;
	GLuint i;
	
	fp = fopen(name, "r");
	if(!fp)
	{
		fprintf(stderr, "The %s file can't open\n", name);
		exit(1);
	}

	nummaterials=0;

	// 計算 material 個數
	while(fscanf(fp, "%s", buf) != EOF)
	{
		switch(buf[0])
		{
		case '#':
			fgets(buf, sizeof(buf), fp);
			break;

		case 'n':			// newmtl 字串
			fgets(buf, sizeof(buf), fp);
			nummaterials++;
			break;

		default:
			fgets(buf, sizeof(buf), fp);
			break;
		}
	}

	rewind(fp);

	model->materials = (ObjMaterial*)malloc(sizeof(ObjMaterial) * nummaterials);
	model->nummaterials = nummaterials;

	// 設定 material 初始值
	for(i=0; i < nummaterials; i++)
	{
		model->materials[i].name = NULL;
		model->materials[i].diffuse[0] = 0.0;
		model->materials[i].diffuse[1] = 0.0;
		model->materials[i].diffuse[2] = 0.0;
		model->materials[i].diffuse[3] = 1.0;

		model->materials[i].ambient[0] = 0.0;
		model->materials[i].ambient[1] = 0.0;
		model->materials[i].ambient[2] = 0.0;
		model->materials[i].ambient[3] = 1.0;

		model->materials[i].specular[0] = 0.0;
		model->materials[i].specular[1] = 0.0;
		model->materials[i].specular[2] = 0.0;
		model->materials[i].specular[3] = 1.0;
		model->materials[i].shininess = 0.0;
		
		model->materials[i].id = NULL;
		model->materials[i].map_name = NULL;
		model->materials[i].bitmap = NULL;
	}

	// 讀取 MTL 資料
	nummaterials = -1;
	while(fscanf(fp, "%s", buf) != EOF)
	{
		switch(buf[0])
		{
		case '#':
			fgets(buf, sizeof(buf), fp);
			break;

		case 'n':			// newmt 名稱
			fgets(buf, sizeof(buf), fp);
			sscanf(buf, "%s %s", buf, buf);
			nummaterials++;
			model->materials[nummaterials].name = strdup(buf);
			break;

		case 'N':
			fscanf(fp, "%f", &model->materials[nummaterials].shininess);
			// 轉換 shininess 值從[0, 1000] -> [0, 128]
			model->materials[nummaterials].shininess /= 1000.0;
			model->materials[nummaterials].shininess *= 128.0;
			break;

		case 'K':
			{
				switch(buf[1])
				{
				case 'd':				// diffuse
					fscanf(fp, "%f %f %f",  &model->materials[nummaterials].diffuse[0],
											&model->materials[nummaterials].diffuse[1],
											&model->materials[nummaterials].diffuse[2]);
					break;
				case 's':				// specular
					fscanf(fp, "%f %f %f",  &model->materials[nummaterials].specular[0],
											&model->materials[nummaterials].specular[1],
											&model->materials[nummaterials].specular[2]);
					break;			
				case 'a':
					fscanf(fp, "%f %f %f",  &model->materials[nummaterials].ambient[0],
											&model->materials[nummaterials].ambient[1],
											&model->materials[nummaterials].ambient[2]);
					break;
				default:
					fgets(buf, sizeof(buf), fp);
					break;
				}
				break;
			}
		case 'm':		// 第8章新增 讀取材質影像檔案
			fgets(buf, sizeof(buf), fp);
			sscanf(buf, "%s %s", buf, buf);
			buf[1] = '/';	// for linux
			model->materials[nummaterials].map_name = strdup(buf);
			break;

		default:
			fgets(buf, sizeof(buf), fp);
			break;
		}
	}

	fclose(fp);

}


// 
// 測試 Wavefront Obj 檔案是否正確，並計算參數值的個數 
//
void ObjFirstPass(ObjModel* model, FILE* fp)
{
	GLuint numvertices = 0;		// 座標點個數
	GLuint numnormals = 0;		// normal個數
	GLuint numtexcoords = 0;	// 貼圖座標個數
	GLuint numtriangles = 0;	// 面的個數
	ObjGroup* group;			// 目前的 Group (最少會有一個Group)
	unsigned v, vt, vn;
	char buf[128];				

	// 新增一個 Group
	group = AddGroup(model, "default");

	while(fscanf(fp, "%s", buf) != EOF)
	{
		switch(buf[0])
		{
		case '#':				// 註解
			fgets(buf, sizeof(buf), fp);
			break;

		case 'v':				// v 第一個視別字
			{
				switch(buf[1])
				{
				case '\0':
					fgets(buf, sizeof(buf), fp);
					numvertices++;
					break;
					
				case 'n':
					fgets(buf, sizeof(buf), fp);
					numnormals++;
					break;

				case 't':
					fgets(buf, sizeof(buf), fp);
					numtexcoords++;
					break;

				default:
					printf("Unknown token\n");
					exit(1);
					break;
				}
			
			}
			break;

		case 'm':				// mtllib	name
			fgets(buf, sizeof(buf), fp);
			sscanf(buf, "%s %s", buf, buf);		// 取得 name
			model->mtllibname = strdup(buf);
			// 讀取 mtl 檔案
			ReadMTL(model, buf);
			break;

		case 'u':
			fgets(buf, sizeof(buf), fp);
			break;

		case 'g':				// Group 
			fgets(buf, sizeof(buf), fp);
			sscanf(buf, "%s", buf);
			group = AddGroup(model, buf);		
			break;

		case 'f':				// face 
			v = vt = vn = 0;
			fscanf(fp, "%d/%d/%d", &v, &vt, &vn);
			fscanf(fp, "%d/%d/%d", &v, &vt, &vn);
			fscanf(fp, "%d/%d/%d", &v, &vt, &vn);
			numtriangles++;
			group->numtriangles++;
			break;

		default:
			fgets(buf, sizeof(buf), fp);
			break;

		}	
	}

	model->numvertices = numvertices;
	model->numnormals = numnormals;
	model->numtexcoords = numtexcoords;
	model->numtriangles = numtriangles;

	group = model->groups;
	while(group)
	{
		group->triangles = (GLuint*)malloc(sizeof(GLuint) * group->numtriangles);
		group->numtriangles = 0;
		group = group->next;
	}
	
}


//
// 測試完後讀取 Wavefront Obj 的資料
//
void ObjSecondPass(ObjModel* model, FILE* fp)
{
	unsigned int i;
	GLuint numvertices = 1;			// 座標點個數
	GLuint numnormals = 1;			// normal個數
	GLuint numtexcoords = 1;		// 貼圖座標個數
	GLuint numtriangles = 0;		// 面的個數
	GLfloat* vertices;				// 存放 vertices
	GLfloat* normals;				// 存放 normals
	GLfloat* texcoords;				// 存放貼圖座標
	ObjGroup* group;				// Group 指標
	
	GLuint material = 0;			// 設定目前 material
	GLuint v, vt, vn;
	char buf[128];

	// 設定指標
	vertices = model->vertices;
	normals = model->normals;
	texcoords = model->texcoords;
	group = model->groups;

	while(fscanf(fp, "%s", buf) != EOF)
	{
		switch(buf[0])
		{
		case '#':
			fgets(buf, sizeof(buf), fp);
			break;

		case 'v':
			{
				switch(buf[1])
				{
				case '\0':
					fscanf(fp, "%f %f %f",
						&vertices[3 * numvertices + 0], 
						&vertices[3 * numvertices + 1],
						&vertices[3 * numvertices + 2]);
					numvertices++;
					break;
				case 'n':
					fscanf(fp, "%f %f %f",
						&normals[3 * numnormals + 0], 
						&normals[3 * numnormals + 1],
						&normals[3 * numnormals + 2]);
					numnormals++;
					break;
				case 't':
					fscanf(fp, "%f %f",
						&texcoords[2 * numtexcoords + 0], 
						&texcoords[2 * numtexcoords + 1]);
					numtexcoords++;
					break;
				}
				break;
			}

		case 'u':				// usemtl
			fgets(buf, sizeof(buf), fp);
			sscanf(buf, "%s %s", buf, buf);
			group->material = material = FindMaterial(model, buf);		// 找尋 material

			break;

		case 'g':				// group
			fgets(buf, sizeof(buf), fp);
			sscanf(buf, "%s", buf);
			group = FindGroup(model, buf);
			break;

		case 'f':
			v = vn = vt = 0;

			fscanf(fp, "%d/%d/%d", &v, &vt, &vn);
			T(numtriangles).vindices[0] = v;
			T(numtriangles).tindices[0] = vt;
			T(numtriangles).nindices[0] = vn;

			fscanf(fp, "%d/%d/%d", &v, &vt, &vn);
			T(numtriangles).vindices[1] = v;
			T(numtriangles).tindices[1] = vt;
			T(numtriangles).nindices[1] = vn;

			fscanf(fp, "%d/%d/%d", &v, &vt, &vn);
			T(numtriangles).vindices[2] = v;
			T(numtriangles).tindices[2] = vt;
			T(numtriangles).nindices[2] = vn;

			group->triangles[group->numtriangles++] = numtriangles;
			numtriangles++;
			while(fscanf(fp, "%d/%d/%d", &v, &vt, &vn) > 0)
			{
				T(numtriangles).vindices[0] = T(numtriangles-1).vindices[0];
				T(numtriangles).tindices[0] = T(numtriangles-1).tindices[0];
				T(numtriangles).nindices[0] = T(numtriangles-1).nindices[0];

				T(numtriangles).vindices[1] = T(numtriangles-1).vindices[2];
				T(numtriangles).tindices[1] = T(numtriangles-1).tindices[2];
				T(numtriangles).nindices[1] = T(numtriangles-1).nindices[2];

				T(numtriangles).vindices[2] = v;
				T(numtriangles).tindices[2] = vt;
				T(numtriangles).nindices[2] = vn;
				group->triangles[group->numtriangles++] = numtriangles;
				numtriangles++;
			}
			break;

		default:
			fgets(buf, sizeof(buf), fp);
			break;

		}
	}

	// 第8章新增 讀取材質影像
	for(i = 0; i < model->nummaterials ;i++)
	{
		if(model->materials[i].map_name)
			LoadBMPTexture(&model->materials[i]);
	}

}

// 
// 讀取 Wavefront Obj 檔案
//
ObjModel* ReadObject(char* filename)
{
	ObjModel* model;
	FILE* fp;

	fp = fopen(filename, "r");
	if(!fp)
	{
		fprintf(stderr, "The %s file can't open\n", filename);
		exit(1);
	}

	// 配置新的 model 資料
	model = (ObjModel*)malloc(sizeof(ObjModel));
	model->mtllibname = NULL;
	model->numvertices = 0;
	model->vertices = NULL;
	model->nummaterials = 0;
	model->materials = NULL;
	model->numtexcoords = 0;
	model->texcoords = NULL;
	model->numtriangles = 0;
	model->triangles = NULL;
	model->nummaterials = 0;
	model->materials = NULL;
	model->numgroups = 0;
	model->groups = NULL;

	// 測試參數是否正確並計算參數值的個數
	ObjFirstPass(model, fp);

	// 配置 vertices、triangles、normals與texcoords
	model->vertices = (GLfloat*)malloc(sizeof(GLfloat) * 3 * (model->numvertices + 1));
	model->triangles = (ObjTriangle*)malloc(sizeof(ObjTriangle) * model->numtriangles);
	
	if(model->numnormals)
		model->normals = (GLfloat*)malloc(sizeof(GLfloat) * 3 * (model->numnormals + 1));

	if(model->numtexcoords)
		model->texcoords = (GLfloat*)malloc(sizeof(GLfloat) * 2 * (model->numtexcoords + 1));


	rewind(fp);

	// 讀取檔案的資料
	ObjSecondPass(model, fp);
	
	fclose(fp);
	return model;
}

//
// 繪出圖型
//
void ObjDraw(ObjModel* model)
{
	static GLuint i;
	static ObjTriangle* triangle;
	static ObjGroup* group;
	static ObjMaterial* material;
	GLuint texflag;

	group = model->groups;
	while(group)
	{
		texflag = false;

		// 設定材質顏色 
		material = &model->materials[group->material];
		if(material)
		{
			glMaterialfv(GL_FRONT, GL_AMBIENT, material->ambient);
			glMaterialfv(GL_FRONT, GL_DIFFUSE, material->diffuse);
			glMaterialfv(GL_FRONT, GL_SPECULAR, material->specular);
			glMaterialf(GL_FRONT, GL_SHININESS, material->shininess);

			// 第8章新增
			if(material->bitmap)
			{
				texflag = true;
				if(!(material->id))
				{
					glGenTextures(1, &material->id);				// 產生 Texture ID
					glBindTexture(GL_TEXTURE_2D, material->id);

					glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
					glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
					glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
					glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);

					// 設定紋理影像的屬性
					glTexImage2D(GL_TEXTURE_2D, 0, 3, 
						 material->width, material->height, 0,
						 GL_BGR_EXT, GL_UNSIGNED_BYTE, material->bitmap);
				}
				else
					glBindTexture(GL_TEXTURE_2D, material->id);

				glEnable(GL_TEXTURE_2D);
			}

		}

		glBegin(GL_TRIANGLES);
		for(i=0; i < group->numtriangles; i++)
		{
			triangle = &T(group->triangles[i]);

			if(texflag)
				glTexCoord2fv(&model->texcoords[2 * triangle->tindices[0]]);
			glNormal3fv(&model->normals[3 * triangle->nindices[0]]);
			glVertex3fv(&model->vertices[3 * triangle->vindices[0]]);

			if(texflag)
				glTexCoord2fv(&model->texcoords[2 * triangle->tindices[1]]);
			glNormal3fv(&model->normals[3 * triangle->nindices[1]]);
			glVertex3fv(&model->vertices[3 * triangle->vindices[1]]);	

			if(texflag)
				glTexCoord2fv(&model->texcoords[2 * triangle->tindices[2]]);
			glNormal3fv(&model->normals[3 * triangle->nindices[2]]);
			glVertex3fv(&model->vertices[3 * triangle->vindices[2]]);

		}
		glEnd();
		glDisable(GL_TEXTURE_2D);

		group = group->next;
	} 
}





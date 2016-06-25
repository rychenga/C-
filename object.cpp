/*
 * Object.cpp
 *	Ū�� Wavefront Obj �ɮסA�����ø�b�ù��W
 *  ��8�� �s�W����v��
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

/*------ Group �H��C���覡�զ� ------*/
// ��M�W�٬� name �� Group
ObjGroup* FindGroup(ObjModel* model, char* name)
{
	ObjGroup* group;
	assert(model);

	group = model->groups;		
	while(group)
	{
		if(!strcmp(name, group->name))		// ���ۦP�W�٫h���}
			break;

		group = group->next;
	}
	return group;
}

/*------ Material ��M ------*/
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

/*------��8���s�W ���oTexture�T�� ------*/
static void LoadBMPTexture(ObjMaterial* material)
{
	BITMAPINFO	texInfo;				// BMP header
	material->bitmap = LoadBitmapFile(material->map_name, &texInfo);
	assert(material->bitmap);

	material->width  = texInfo.bmiHeader.biWidth;
	material->height = texInfo.bmiHeader.biHeight;
}

// �[�J�W�٬� name �� Group
ObjGroup* AddGroup(ObjModel* model, char* name)
{
	ObjGroup* group;
	group = FindGroup(model, name);

	// ���p�S�����~�s�W
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
//	Ū�� MTL �ɮפ�����Ʀs�J 
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

	// �p�� material �Ӽ�
	while(fscanf(fp, "%s", buf) != EOF)
	{
		switch(buf[0])
		{
		case '#':
			fgets(buf, sizeof(buf), fp);
			break;

		case 'n':			// newmtl �r��
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

	// �]�w material ��l��
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

	// Ū�� MTL ���
	nummaterials = -1;
	while(fscanf(fp, "%s", buf) != EOF)
	{
		switch(buf[0])
		{
		case '#':
			fgets(buf, sizeof(buf), fp);
			break;

		case 'n':			// newmt �W��
			fgets(buf, sizeof(buf), fp);
			sscanf(buf, "%s %s", buf, buf);
			nummaterials++;
			model->materials[nummaterials].name = strdup(buf);
			break;

		case 'N':
			fscanf(fp, "%f", &model->materials[nummaterials].shininess);
			// �ഫ shininess �ȱq[0, 1000] -> [0, 128]
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
		case 'm':		// ��8���s�W Ū������v���ɮ�
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
// ���� Wavefront Obj �ɮ׬O�_���T�A�íp��ѼƭȪ��Ӽ� 
//
void ObjFirstPass(ObjModel* model, FILE* fp)
{
	GLuint numvertices = 0;		// �y���I�Ӽ�
	GLuint numnormals = 0;		// normal�Ӽ�
	GLuint numtexcoords = 0;	// �K�Ϯy�ЭӼ�
	GLuint numtriangles = 0;	// �����Ӽ�
	ObjGroup* group;			// �ثe�� Group (�ַ̤|���@��Group)
	unsigned v, vt, vn;
	char buf[128];				

	// �s�W�@�� Group
	group = AddGroup(model, "default");

	while(fscanf(fp, "%s", buf) != EOF)
	{
		switch(buf[0])
		{
		case '#':				// ����
			fgets(buf, sizeof(buf), fp);
			break;

		case 'v':				// v �Ĥ@�ӵ��O�r
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
			sscanf(buf, "%s %s", buf, buf);		// ���o name
			model->mtllibname = strdup(buf);
			// Ū�� mtl �ɮ�
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
// ���է���Ū�� Wavefront Obj �����
//
void ObjSecondPass(ObjModel* model, FILE* fp)
{
	unsigned int i;
	GLuint numvertices = 1;			// �y���I�Ӽ�
	GLuint numnormals = 1;			// normal�Ӽ�
	GLuint numtexcoords = 1;		// �K�Ϯy�ЭӼ�
	GLuint numtriangles = 0;		// �����Ӽ�
	GLfloat* vertices;				// �s�� vertices
	GLfloat* normals;				// �s�� normals
	GLfloat* texcoords;				// �s��K�Ϯy��
	ObjGroup* group;				// Group ����
	
	GLuint material = 0;			// �]�w�ثe material
	GLuint v, vt, vn;
	char buf[128];

	// �]�w����
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
			group->material = material = FindMaterial(model, buf);		// ��M material

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

	// ��8���s�W Ū������v��
	for(i = 0; i < model->nummaterials ;i++)
	{
		if(model->materials[i].map_name)
			LoadBMPTexture(&model->materials[i]);
	}

}

// 
// Ū�� Wavefront Obj �ɮ�
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

	// �t�m�s�� model ���
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

	// ���հѼƬO�_���T�íp��ѼƭȪ��Ӽ�
	ObjFirstPass(model, fp);

	// �t�m vertices�Btriangles�Bnormals�Ptexcoords
	model->vertices = (GLfloat*)malloc(sizeof(GLfloat) * 3 * (model->numvertices + 1));
	model->triangles = (ObjTriangle*)malloc(sizeof(ObjTriangle) * model->numtriangles);
	
	if(model->numnormals)
		model->normals = (GLfloat*)malloc(sizeof(GLfloat) * 3 * (model->numnormals + 1));

	if(model->numtexcoords)
		model->texcoords = (GLfloat*)malloc(sizeof(GLfloat) * 2 * (model->numtexcoords + 1));


	rewind(fp);

	// Ū���ɮת����
	ObjSecondPass(model, fp);
	
	fclose(fp);
	return model;
}

//
// ø�X�ϫ�
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

		// �]�w�����C�� 
		material = &model->materials[group->material];
		if(material)
		{
			glMaterialfv(GL_FRONT, GL_AMBIENT, material->ambient);
			glMaterialfv(GL_FRONT, GL_DIFFUSE, material->diffuse);
			glMaterialfv(GL_FRONT, GL_SPECULAR, material->specular);
			glMaterialf(GL_FRONT, GL_SHININESS, material->shininess);

			// ��8���s�W
			if(material->bitmap)
			{
				texflag = true;
				if(!(material->id))
				{
					glGenTextures(1, &material->id);				// ���� Texture ID
					glBindTexture(GL_TEXTURE_2D, material->id);

					glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
					glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
					glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
					glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);

					// �]�w���z�v�����ݩ�
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





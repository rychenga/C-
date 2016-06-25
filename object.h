/*
 *	Object.h
 *		Wavefront Obj的資料結構 （第2章）
 *
 */
#include <GL/glut.h>


//
// 定義材質對光線的資料結構
//
typedef struct _ObjMaterial
{
	char*			name;					// 材質的名稱
	GLfloat			diffuse[4];				// 散射光
	GLfloat			ambient[4];				// 環境光
	GLfloat			specular[4];			// 反射光
		 
	GLfloat			shininess;				// 對光的反射強度
	char*			map_name;				// 材質貼圖的名稱
	
	GLuint			id;						// texture id
	GLubyte*		bitmap;					// 圖形指標
	GLint			width;					// 圖形寬
	GLint			height;					// 圖形高
}ObjMaterial;

// 
// 定義 Triangle 的資料結構
//
typedef struct _ObjTriangle
{
	GLuint vindices[3];
	GLuint nindices[3];
	GLuint tindices[3];
}ObjTriangle;

// 
// 定義 Group 的資料結構
//
typedef struct _ObjGroup
{
	char*			name;					// Group名稱
	GLuint			numtriangles;			// 此 Group 由多少個三角形組成
	GLuint*			triangles;				// 指向三角形的陣列指標
	GLuint			material;				// 使用的material
	struct _ObjGroup* next;					// 指向下一個 Group

}ObjGroup;


//
// 定義立體模型的資料結構
//
typedef struct _ObjModel
{
	char*			mtllibname;				// mtl 檔案名稱
	
	GLuint			numvertices;			// vertice 的數量
	GLfloat*		vertices;				// 收集所有 vertice 的陣列

	GLuint			numtexcoords;			// 貼圖座標的數量
	GLfloat*		texcoords;				// 貼圖座標的陣列
	
	GLuint			numnormals;				// normal 的數量
	GLfloat*		normals;				// normal 的陣列

	GLuint			numtriangles;			// 圖形由多少個三角形組成
	ObjTriangle*	triangles;				// 存放三角形的陣列

	GLuint			nummaterials;			// 圖形有多少個材質貼圖
	ObjMaterial*	materials;				// 存放材質貼圖的陣列

	GLuint			numgroups;				// 圖形有多少個Group組成
	ObjGroup*		groups;					// 連結每個 Group

} ObjModel;


ObjModel* ReadObject(char* filename);
void ObjDraw(ObjModel* model);

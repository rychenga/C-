/*
 *	Object.h
 *		Wavefront Obj����Ƶ��c �]��2���^
 *
 */
#include <GL/glut.h>


//
// �w�q�������u����Ƶ��c
//
typedef struct _ObjMaterial
{
	char*			name;					// ���誺�W��
	GLfloat			diffuse[4];				// ���g��
	GLfloat			ambient[4];				// ���ҥ�
	GLfloat			specular[4];			// �Ϯg��
		 
	GLfloat			shininess;				// ������Ϯg�j��
	char*			map_name;				// ����K�Ϫ��W��
	
	GLuint			id;						// texture id
	GLubyte*		bitmap;					// �ϧΫ���
	GLint			width;					// �ϧμe
	GLint			height;					// �ϧΰ�
}ObjMaterial;

// 
// �w�q Triangle ����Ƶ��c
//
typedef struct _ObjTriangle
{
	GLuint vindices[3];
	GLuint nindices[3];
	GLuint tindices[3];
}ObjTriangle;

// 
// �w�q Group ����Ƶ��c
//
typedef struct _ObjGroup
{
	char*			name;					// Group�W��
	GLuint			numtriangles;			// �� Group �Ѧh�֭ӤT���βզ�
	GLuint*			triangles;				// ���V�T���Ϊ��}�C����
	GLuint			material;				// �ϥΪ�material
	struct _ObjGroup* next;					// ���V�U�@�� Group

}ObjGroup;


//
// �w�q����ҫ�����Ƶ��c
//
typedef struct _ObjModel
{
	char*			mtllibname;				// mtl �ɮצW��
	
	GLuint			numvertices;			// vertice ���ƶq
	GLfloat*		vertices;				// �����Ҧ� vertice ���}�C

	GLuint			numtexcoords;			// �K�Ϯy�Ъ��ƶq
	GLfloat*		texcoords;				// �K�Ϯy�Ъ��}�C
	
	GLuint			numnormals;				// normal ���ƶq
	GLfloat*		normals;				// normal ���}�C

	GLuint			numtriangles;			// �ϧΥѦh�֭ӤT���βզ�
	ObjTriangle*	triangles;				// �s��T���Ϊ��}�C

	GLuint			nummaterials;			// �ϧΦ��h�֭ӧ���K��
	ObjMaterial*	materials;				// �s�����K�Ϫ��}�C

	GLuint			numgroups;				// �ϧΦ��h�֭�Group�զ�
	ObjGroup*		groups;					// �s���C�� Group

} ObjModel;


ObjModel* ReadObject(char* filename);
void ObjDraw(ObjModel* model);

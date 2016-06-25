/*
 * ShowObj.cpp
 *	測試 ReadObject 與 ObjDraw 這兩個函式
 */
#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <GL/glut.h>
#include "object.h"

#define TITLE	"8.5節的範例 使用材質貼圖"
ObjModel* pmodel = NULL;
GLfloat light_position[] = {12.0f, 8.0f, 42.0f, 1.0f};

GLfloat old_x, old_y, rotate_x=0, rotate_y=0;
//	設定發光體的光源模型
void SetLightSource()
{
	GLfloat light_specular[] = { 1.0f, 1.0f, 1.0f, 1.0f};
	GLfloat light_diffuse[] = { 1.0f, 1.0f, 1.0f, 1.0f};
	GLfloat light_ambient[] = { 1.0f, 1.0f, 1.0f, 1.0f};

	glEnable(GL_LIGHTING);

	// 設定發光體的光源的特性
	glLightfv(GL_LIGHT0,GL_AMBIENT,light_ambient);		// 環境光(Ambient Light)
	glLightfv(GL_LIGHT0,GL_DIFFUSE,light_diffuse);		// 散射光(Diffuse Light)
	glLightfv(GL_LIGHT0,GL_SPECULAR,light_specular);	// 反射光(Specular Light)

	glEnable(GL_LIGHT0);	
	glEnable(GL_DEPTH_TEST);
}



void OnDraw(void)
{
	glClearColor(0.0, 0.0, 0.0, 1.0);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glLoadIdentity();
	
	gluLookAt(60.0f, 60.0f, 60.0f,
			  0.0f, 10.0f, 0.0f,
 			  0.0f, 2.0f, 0.0f);

	glLightfv(GL_LIGHT0,GL_POSITION,light_position);
	// 繪製物體
	glPushMatrix();
	glTranslatef(0, 0, 0);
	glRotatef(rotate_y, 1.0, 0.0, 0.0);
    glRotatef(rotate_x, 0.0, 1.0, 0.0);

	ObjDraw(pmodel);
	glPopMatrix();


	glutSwapBuffers();
}

void OnSize(int w, int h)
{
    glViewport(0, 0, w, h);

	if(h == 0)
		h=1;
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
	
	gluPerspective(45.0f, w/h, 1.0, 425.0);

    glMatrixMode(GL_MODELVIEW);
}


void OnKey(unsigned char key, int x, int y)
{
	switch(key)
	{
	case 27:
		exit(0);
		break;
	}
}

void OnMouse(int button, int state, int x, int y)
{
	old_x = x;
	old_y = y;
}

void OnMotion(int x, int y)
{
	rotate_x = x - old_x;
	rotate_y = y - old_y;
	glutPostRedisplay();
}


int main(int argc, char** argv)
{
	glutInit(&argc, argv);

	// 設定視窗使用模式、視窗大小、座標
	glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB |GLUT_DEPTH);
	glutInitWindowSize(300,300);
	glutInitWindowPosition(50, 50);
	glutCreateWindow(TITLE);

	pmodel = ReadObject("5.obj");			// 讀取立體模型

	glutReshapeFunc(OnSize);
	glutDisplayFunc(OnDraw);
	glutKeyboardFunc(OnKey);
	glutMouseFunc(OnMouse);
	glutMotionFunc(OnMotion);
	
	SetLightSource();

	glutMainLoop();
	return 0;
}

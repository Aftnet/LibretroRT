#pragma once

using namespace AngleSupport;

namespace AngleApp
{
	public ref class SimpleRenderer sealed : IRenderer
    {
    public:
        SimpleRenderer();
        virtual ~SimpleRenderer();
		virtual void Init();
		virtual void Deinit();
        virtual void Draw();
        virtual void UpdateWindowSize(GLsizei width, GLsizei height);

    private:
        GLuint mProgram;
        GLsizei mWindowWidth;
        GLsizei mWindowHeight;

        GLint mPositionAttribLocation;
        GLint mColorAttribLocation;

        GLint mModelUniformLocation;
        GLint mViewUniformLocation;
        GLint mProjUniformLocation;

        GLuint mVertexPositionBuffer;
        GLuint mVertexColorBuffer;
        GLuint mIndexBuffer;

        int mDrawCount;
    };
}
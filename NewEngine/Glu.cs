using OpenTK;
using OpenTK.Graphics.OpenGL;

public static class Glu
{
    public static int UnProject(Vector3 win, ref Vector3 obj)
    {
        Matrix4 modelMatrix;
        GL.GetFloat(GetPName.ModelviewMatrix, out modelMatrix);

        Matrix4 projMatrix;
        GL.GetFloat(GetPName.ProjectionMatrix, out projMatrix);

        int[] viewport = new int[4];
        GL.GetInteger(GetPName.Viewport, viewport);

        return UnProject(win, modelMatrix, projMatrix, viewport, ref obj);
    }

    public static int UnProject(Vector3 win, Matrix4 modelMatrix, Matrix4 projMatrix, int[] viewport, ref Vector3 obj)
    {
        return gluUnProject(win.X, win.Y, win.Z, modelMatrix, projMatrix, viewport, ref obj.X, ref obj.Y, ref obj.Z);
    }

    private static int gluUnProject(float winx, float winy, float winz, Matrix4 modelMatrix, Matrix4 projMatrix, int[] viewport, ref float objx, ref float objy, ref float objz)
    {
        Matrix4 finalMatrix;
        Vector4 _in;
        Vector4 _out;

        finalMatrix = Matrix4.Mult(modelMatrix, projMatrix);

        //if (!__gluInvertMatrixd(finalMatrix, finalMatrix)) return(GL_FALSE);
        finalMatrix.Invert();

        _in.X = winx;
        _in.Y = viewport[3] - winy;
        _in.Z = winz;
        _in.W = 1.0f;

        /* Map x and y from window coordinates */
        _in.X = (_in.X - viewport[0]) / viewport[2];
        _in.Y = (_in.Y - viewport[1]) / viewport[3];

        /* Map to range -1 to 1 */
        _in.X = _in.X * 2 - 1;
        _in.Y = _in.Y * 2 - 1;
        _in.Z = _in.Z * 2 - 1;

        //__gluMultMatrixVecd(finalMatrix, _in, _out);
        // check if this works:
        _out = Vector4.Transform(_in, finalMatrix);

        if (_out.W == 0.0)
            return (0);
        _out.X /= _out.W;
        _out.Y /= _out.W;
        _out.Z /= _out.W;
        objx = _out.X;
        objy = _out.Y;
        objz = _out.Z;
        return (1);
    }
}
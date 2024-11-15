using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using Sauce_Engine;
using Sauce_Engine.Types;
using Sauce_Engine.Util;

namespace Sauce_Engine;

// In this tutorial we focus on how to set up a scene with multiple lights, both of different types but also
// with several point lights
public class Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private readonly float[] _vertices =
    {
        // Positions          Normals              Texture coords
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
        0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
        0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

        0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
        0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
        0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
        0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
        0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
        0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
        0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
        0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
        0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
    };

    private MapObjList mapObjects = MapObjList.Instance;
    private MapObjList imObjects = new();

    // We need the point lights' positions to draw the lamps and to get light the materials properly
    // MUST ADJUST THE PREPROCESSOR DIRECTIVE NR_POINT_LIGHTS TO LIGHT COUNT

    private int _vertexBufferObject;

    private int _vaoModel;

    private int _vaoLamp;

    private Shader _lampShader;

    private Shader _lightingShader;
    TextureManager texMan;

    private Camera _camera;

    private Movement movement;

    private bool _firstMove = true;

    private Vector2 _lastPos;

    protected override void OnLoad()
    {
        base.OnLoad();

        GLFW.WindowHint(WindowHintInt.Samples, 4);        

        GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
        _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

        {
            _vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(_vaoModel);

            var positionLocation = _lightingShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var normalLocation = _lightingShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        }

        {
            _vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(_vaoLamp);

            var positionLocation = _lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        }

        texMan = new(_lightingShader);

        _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

        CursorState = CursorState.Grabbed;

        movement = new(_camera, mapObjects, imObjects);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindVertexArray(_vaoModel);

        texMan.SwapToTexture(Textures.Crate);
        mapObjects.eyepos = _camera.Position;
        imObjects.eyepos = _camera.Position;

        _lightingShader.Use();

        _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
        _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

        _lightingShader.SetVector3("viewPos", _camera.Position);

        _lightingShader.SetInt("material.diffuse", 0);
        _lightingShader.SetInt("material.specular", 1);
        _lightingShader.SetInt("material.normal", 2);
        _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
        _lightingShader.SetFloat("material.shininess", 32.0f);

        /*
           Here we set all the uniforms for the 5/6 types of lights we have. We have to set them manually and index
           the proper PointLight struct in the array to set each uniform variable. This can be done more code-friendly
           by defining light types as classes and set their values in there, or by using a more efficient uniform approach
           by using 'Uniform buffer objects', but that is something we'll discuss in the 'Advanced GLSL' tutorial.
        */
        // Directional light
        _lightingShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
        _lightingShader.SetVector3("dirLight.ambient", new Vector3(0.2f, 0.2f, 0.2f));
        _lightingShader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
        _lightingShader.SetVector3("dirLight.specular", new Vector3(0.1f, 0.1f, 0.1f));

        // Setting the point and spot lights
        int oLightCt = 0;
        int dLightCt = 0;
        foreach (var obj in mapObjects)
        {
            if (obj is Entity ent)
            {
                switch (ent.Type)
                {
                    case Enums.EntityType.OmniLight:
                        _lightingShader.SetVector3($"pointLights[{oLightCt}].position", ent.transform.Translation);
                        _lightingShader.SetVector3($"pointLights[{oLightCt}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                        _lightingShader.SetVector3($"pointLights[{oLightCt}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                        _lightingShader.SetVector3($"pointLights[{oLightCt}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                        _lightingShader.SetFloat($"pointLights[{oLightCt}].constant", 1.0f);
                        _lightingShader.SetFloat($"pointLights[{oLightCt}].linear", 0.09f);
                        _lightingShader.SetFloat($"pointLights[{oLightCt}].quadratic", 0.032f);
                        oLightCt++;
                        break;
                    case Enums.EntityType.DirectLight:
                        _lightingShader.SetVector3($"spotLights[{dLightCt}].position", ent.transform.Translation);
                        _lightingShader.SetVector3($"spotLights[{dLightCt}].direction", ent.transform.Rotation);
                        _lightingShader.SetVector3($"spotLights[{dLightCt}].ambient", new Vector3(0.0f, 0.0f, 0.0f));
                        _lightingShader.SetVector3($"spotLights[{dLightCt}].diffuse", new Vector3(1.0f, 1.0f, 1.0f));
                        _lightingShader.SetVector3($"spotLights[{dLightCt}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                        _lightingShader.SetFloat($"spotLights[{dLightCt}].constant", 1.0f);
                        _lightingShader.SetFloat($"spotLights[{dLightCt}].linear", 0.09f);
                        _lightingShader.SetFloat($"spotLights[{dLightCt}].quadratic", 0.032f);
                        _lightingShader.SetFloat($"spotLights[{dLightCt}].cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
                        _lightingShader.SetFloat($"spotLights[{dLightCt}].outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
                        dLightCt++;
                        break;
                }
            }
        }

        foreach (var mapobj in mapObjects)
        {
            if (mapobj is Brush b)
            {
                texMan.SwapToTexture(b.texture);
                Matrix4 model = b.MakeGlModelMat();
                _lightingShader.SetMatrix4("model", model);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
        }

        GL.BindVertexArray(_vaoLamp);

        _lampShader.Use();

        _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
        _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

        // Drawing all the light boxes for the lights
        foreach (var mapobj in mapObjects)
        {
            if (mapobj is Entity ent)
            {
                Matrix4 model = ent.MakeGlModelMat();
                _lampShader.SetMatrix4("model", model);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            }
        }

        // Drawing the immidiate brushes before disposing of them
        foreach(var mapobj in imObjects)
        {
            if (mapobj is Brush b)
            {
                Matrix4 model = b.MakeGlModelMat();
                _lightingShader.SetMatrix4("model", model);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
        }
        imObjects.Clear();

        SwapBuffers();
    }



    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        if (!IsFocused) return;
        var input = KeyboardState;
        if (input.IsKeyDown(Keys.Escape)) Close();

        movement.Process(input, e);
        // Just making sure the player never falls OOB
        if (_camera.Position.Y < -50) _camera.Position = new(0, 10, 0);

        // Camera and movement stuff
        const float sensitivity = 0.2f;     
        var mouse = MouseState;

        if (_firstMove)
        {
            _lastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
        }
        else
        {
            var deltaX = mouse.X - _lastPos.X;
            var deltaY = mouse.Y - _lastPos.Y;
            _lastPos = new Vector2(mouse.X, mouse.Y);

            _camera.Yaw += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity;
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _camera.Fov -= e.OffsetY;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
        _camera.AspectRatio = Size.X / (float)Size.Y;
    }
}
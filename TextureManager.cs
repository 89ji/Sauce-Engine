using OpenTK.Graphics.OpenGL4;
using Sauce_Engine;

class TextureManager
{
    Shader lShader;
    Dictionary<Textures, (Texture, Texture)> TexMap;
    public TextureManager(Shader lShader)
    {
        TexMap = new();
        this.lShader = lShader;

        Texture concFloor = Texture.LoadFromFile(@"Resources/concrete_tile.jpg");
        Texture concFloorSpec = Texture.LoadFromFile(@"Resources/concrete_tile spec.jpg");
        TexMap.Add(Textures.ConcFloor, (concFloor, concFloorSpec));

        Texture concWall = Texture.LoadFromFile(@"Resources/concrete wall.jpg");
        Texture concWallSpec = Texture.LoadFromFile(@"Resources/concrete wall spec.jpg");
        TexMap.Add(Textures.ConcWall, (concWall, concWallSpec));

        Texture glass = Texture.LoadFromFile(@"Resources/glass.jpg");
        TexMap.Add(Textures.Glass, (glass, glass));

        // TODO dark and light concrete

        Texture metal = Texture.LoadFromFile(@"Resources/metal.png");
        Texture metalSpec = Texture.LoadFromFile(@"Resources/metal spec.png");
        TexMap.Add(Textures.Metal, (metal, metalSpec));

        Texture brick = Texture.LoadFromFile(@"Resources/outer tile.png");
        Texture brickSpec = Texture.LoadFromFile(@"Resources/outer tile spec.png");
        TexMap.Add(Textures.Brick, (brick, brickSpec));

        /*
        TODO indoors concrete floor
        Texture inConc = Texture.LoadFromFile(@"Resources/outer tile.png");
        Texture inConcSpec = Texture.LoadFromFile(@"Resources/outer tile spec.png");
        TexMap.Add(Textures.IndoorConcFloor, (inConc, inConcSpec));
        */
        
        Texture carpet = Texture.LoadFromFile(@"Resources/carpet.jpg");
        Texture carpetSpec = Texture.LoadFromFile(@"Resources/carpet spec.jpg");
        TexMap.Add(Textures.Carpet, (carpet, carpetSpec));

        Texture ceiling = Texture.LoadFromFile(@"Resources/ceiling.jpg");
        Texture ceilingSpec = Texture.LoadFromFile(@"Resources/ceiling spec.jpg");
        TexMap.Add(Textures.Ceiling, (ceiling, ceilingSpec));

        // TODO brick wall

        // TODO dirt

        Texture crate = Texture.LoadFromFile(@"Resources/crate.png");
        Texture crateSpec = Texture.LoadFromFile(@"Resources/crate spec.png");
        TexMap.Add(Textures.Crate, (crate, crateSpec));

    }

    public void SwapToTexture(Textures target)
    {
        if (!TexMap.ContainsKey(target)) throw new NotImplementedException("Texture has not been addded to map yet!");
        var textures = TexMap[target];
        textures.Item1.Use(TextureUnit.Texture0);
        textures.Item2.Use(TextureUnit.Texture1);
    }
}

public enum Textures
{
    ConcFloor,
    ConcWall,
    Glass,
    ConcDark,
    ConcLight,
    Metal,
    Brick,
    IndoorConcFloor,
    Carpet,
    Ceiling,
    BrickWall,
    Dirt,
    Crate
}
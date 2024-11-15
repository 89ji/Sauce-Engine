using OpenTK.Graphics.OpenGL4;
using Sauce_Engine;

class TextureManager
{
    Shader lShader;
    Dictionary<Textures, (Texture, Texture)> TexMap;
    Dictionary<Textures, Texture> NormalMap;
    Texture DefaultNormal;
    public TextureManager(Shader lShader)
    {
        TexMap = new();
        NormalMap = new();
        this.lShader = lShader;

        Texture concFloor = Texture.LoadFromFile(@"Resources/concrete_tile.jpg");
        Texture concFloorSpec = Texture.LoadFromFile(@"Resources/concrete_tile spec.jpg");
        Texture concFloorNormal = Texture.LoadFromFile(@"Resources/concrete_tile normal.png");
        TexMap.Add(Textures.ConcFloor, (concFloor, concFloorSpec));
        NormalMap.Add(Textures.ConcFloor, concFloorNormal);

        Texture concWall = Texture.LoadFromFile(@"Resources/concrete wall.jpg");
        Texture concWallSpec = Texture.LoadFromFile(@"Resources/concrete wall spec.jpg");
        TexMap.Add(Textures.ConcWall, (concWall, concWallSpec));

        Texture glass = Texture.LoadFromFile(@"Resources/glass w trans.png");
        TexMap.Add(Textures.Glass, (glass, glass));

        Texture concDark = Texture.LoadFromFile(@"Resources/conc dark.jpg");
        Texture concLight = Texture.LoadFromFile(@"Resources/conc light.jpg");
        Texture concSpec = Texture.LoadFromFile(@"Resources/metal spec.png");
        TexMap.Add(Textures.ConcDark, (concDark, concSpec));
        TexMap.Add(Textures.ConcLight, (concLight, concSpec));


        Texture metal = Texture.LoadFromFile(@"Resources/metal.png");
        Texture metalSpec = Texture.LoadFromFile(@"Resources/metal spec.png");
        TexMap.Add(Textures.Metal, (metal, metalSpec));

        Texture brick = Texture.LoadFromFile(@"Resources/outer tile.png");
        Texture brickSpec = Texture.LoadFromFile(@"Resources/outer tile spec.png");
        TexMap.Add(Textures.Brick, (brick, brickSpec));

        Texture inConc = Texture.LoadFromFile(@"Resources/inConcFloor.jpg");
        Texture inConcSpec = Texture.LoadFromFile(@"Resources/inConcFloorSpec.jpg");
        TexMap.Add(Textures.IndoorConcFloor, (inConc, inConcSpec));

        Texture carpet = Texture.LoadFromFile(@"Resources/carpet.jpg");
        Texture carpetSpec = Texture.LoadFromFile(@"Resources/carpet spec.jpg");
        TexMap.Add(Textures.Carpet, (carpet, carpetSpec));

        Texture ceiling = Texture.LoadFromFile(@"Resources/ceiling.jpg");
        Texture ceilingSpec = Texture.LoadFromFile(@"Resources/ceiling spec.jpg");
        Texture ceilingNormal = Texture.LoadFromFile(@"Resources/ceiling normal.png");
        TexMap.Add(Textures.Ceiling, (ceiling, ceilingSpec));
        NormalMap.Add(Textures.Ceiling, ceilingNormal);

        Texture brickWall = Texture.LoadFromFile(@"Resources/brick wall.jpg");
        Texture brickWallSpec = Texture.LoadFromFile(@"Resources/brick wall spec.jpg");
        TexMap.Add(Textures.BrickWall, (brickWall, brickWallSpec));

        Texture dirt = Texture.LoadFromFile(@"Resources/dirt.jpg");
        Texture dirtSpec = Texture.LoadFromFile(@"Resources/dirt spec.jpg");
        TexMap.Add(Textures.Dirt, (dirt, dirtSpec));

        Texture crate = Texture.LoadFromFile(@"Resources/crate.png");
        Texture crateSpec = Texture.LoadFromFile(@"Resources/crate spec.png");
        Texture crateNormal = Texture.LoadFromFile(@"Resources/crate normal.png");
        TexMap.Add(Textures.Crate, (crate, crateSpec));
        NormalMap.Add(Textures.Crate, crateNormal);


        DefaultNormal = Texture.LoadFromFile(@"Resources/default normal.png");
    }

    public void SwapToTexture(Textures target)
    {
        if (!TexMap.ContainsKey(target)) throw new NotImplementedException("Texture has not been addded to map yet!");
        var textures = TexMap[target];
        textures.Item1.Use(TextureUnit.Texture0);
        textures.Item2.Use(TextureUnit.Texture1);

        if (NormalMap.ContainsKey(target)) NormalMap[target].Use(TextureUnit.Texture2);
        else DefaultNormal.Use(TextureUnit.Texture2);
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
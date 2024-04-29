using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public static class SteamHelpers
{
    public enum AvatarSize { Small, Medium, Large };
    private static Dictionary<(CSteamID, AvatarSize), Texture2D> avatarTextures = new Dictionary<(CSteamID, AvatarSize), Texture2D>();

    // Callbacks
    private static Callback<PersonaStateChange_t> personaStateChange = Callback<PersonaStateChange_t>.Create(PersonaStateChange);
    private static Callback<AvatarImageLoaded_t> avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(AvatarImageLoaded);

    private static void AvatarImageLoaded(AvatarImageLoaded_t param) { }
    private static void PersonaStateChange(PersonaStateChange_t param) { }

    public static Texture2D GetAvatarTexture(CSteamID id, AvatarSize size = AvatarSize.Large)
    {
        if(id == CSteamID.Nil) return null;

        if(avatarTextures.ContainsKey((id, size)))
            return avatarTextures[(id, size)];

        int avatar_id = 0;
        switch(size)
        {
            case AvatarSize.Small:
                avatar_id = SteamFriends.GetSmallFriendAvatar(id);
                break;
            case AvatarSize.Medium:
                avatar_id = SteamFriends.GetMediumFriendAvatar(id);
                break;
            case AvatarSize.Large:
                avatar_id = SteamFriends.GetLargeFriendAvatar(id);
                break;
        }

        Texture2D avatar = null;
        
        if (SteamUtils.GetImageSize(avatar_id, out uint width, out uint height) == true && width > 0 && height > 0)
        {
            var image = new byte[width * height * 4];
            SteamUtils.GetImageRGBA(avatar_id, image, (int)(width * height * 4));
            avatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
            avatar.LoadRawTextureData(image);
            avatar.Apply();

            // flip the texture vertically
            var pixels = avatar.GetPixels();
            Array.Reverse(pixels, 0, pixels.Length);
            avatar.SetPixels(pixels);
            avatar.Apply();

            // flip the texture horizontally
            pixels = avatar.GetPixels();
            for (int i = 0; i < avatar.width; i++)
                Array.Reverse(pixels, i * avatar.width, avatar.width);
            avatar.SetPixels(pixels);
            avatar.Apply();

            // Convert RGBA to sRGB
            pixels = avatar.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].r = Mathf.GammaToLinearSpace(pixels[i].r);
                pixels[i].g = Mathf.GammaToLinearSpace(pixels[i].g);
                pixels[i].b = Mathf.GammaToLinearSpace(pixels[i].b);
            }
            avatar.SetPixels(pixels);
            avatar.Apply();
        }

        avatarTextures.Add((id, size), avatar);

        return avatar;
    }

    public static Sprite GetAvatarSprite(CSteamID id, AvatarSize size = AvatarSize.Large)
    {
        var texture = GetAvatarTexture(id, size);

        if(texture == null) return null;

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    
    public static string GetPersonaName() => SteamFriends.GetPersonaName();
    public static string GetPersonaName(CSteamID id) => SteamFriends.GetFriendPersonaName(id);

    public static string GetPersonaState() => SteamFriends.GetPersonaState().ToString();
    public static string GetPersonaState(CSteamID id) => SteamFriends.GetFriendPersonaState(id).ToString();

    public static CSteamID GetSteamID() => SteamUser.GetSteamID();

    public static CSteamID ConvertToCSteamID<T>(T id)
    {
        if(id is ulong)
            return new CSteamID((ulong)(object)id);
        else if(id is string)
            return new CSteamID(ulong.Parse((string)(object)id));
        else
            throw new ArgumentException("Invalid type");
    }
}

//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using System.Drawing;

namespace GTA
{
    public struct ParticleEffectAsset : IEquatable<ParticleEffectAsset>
    {
        /// <summary>
        /// Creates a class used for loading <see cref="ParticleEffectAsset"/>s than can be used to start <see cref="ParticleEffect"/>s from inside the Asset
        /// </summary>
        /// <param name="assetName">The name of the asset file which contains all the <see cref="ParticleEffect"/>s you are wanting to start</param>
        /// <remarks>The files have the extension *.ypt in OpenIV, use the file name withouth the extension for the <paramref name="assetName"/></remarks>
        public ParticleEffectAsset(string assetName)
        {
            AssetName = assetName;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ParticleEffectAsset"/> is Loaded
        /// </summary>
        /// <remarks>Use <see cref="Request()"/> or <see cref="Request(int)"/> to load the asset</remarks>
        public bool IsLoaded => Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, AssetName);

        /// <summary>
        /// Gets the name of the this <see cref="ParticleEffectAsset"/> file.
        /// </summary>
        public string AssetName { get; }

        /// <summary>
        /// Sets the <see cref="Color"/> for all NonLooped Particle Effects
        /// </summary>
        public static Color NonLoopedColor
        {
            set
            {
                Call(Hash.SET_PARTICLE_FX_NON_LOOPED_COLOUR, value.R / 255f, value.G / 255f, value.B / 255f);
                Call(Hash.SET_PARTICLE_FX_NON_LOOPED_ALPHA, value.A / 255f);
            }
        }

        /// <summary>
        /// Make this the current particle asset.
        /// </summary>
        internal bool UseNext()
        {
            Request();
            if (IsLoaded)
            {
                Call(Hash.USE_PARTICLE_FX_ASSET, AssetName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to load this <see cref="ParticleEffectAsset"/> into memory so it can be used for starting <see cref="ParticleEffect"/>s.
        /// </summary>
        public void Request()
        {
            Call(Hash.REQUEST_NAMED_PTFX_ASSET, AssetName);
        }

        /// <summary>
        /// Attempts to load this <see cref="ParticleEffectAsset"/> into memory so it can be used for starting <see cref="ParticleEffect"/>s.
        /// </summary>
        /// <param name="timeout">How long in milliseconds should the game wait while the model hasn't been loaded before giving up</param>
        /// <returns><see langword="true" /> if the <see cref="ParticleEffectAsset"/> is Loaded; otherwise, <see langword="false" /></returns>
        public bool Request(int timeout)
        {
            Request();

            DateTime endtime = timeout >= 0 ? DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, timeout) : DateTime.MaxValue;

            while (!IsLoaded)
            {
                Script.Yield();
                Request();

                if (DateTime.UtcNow >= endtime)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Tells the game we have finished using this <see cref="ParticleEffectAsset"/> and it can be freed from memory.
        /// </summary>
        public void MarkAsNoLongerNeeded()
        {
            Call(Hash.REMOVE_NAMED_PTFX_ASSET, AssetName);
        }

        public bool Equals(ParticleEffectAsset asset)
        {
            return AssetName == asset.AssetName;
        }

        public override bool Equals(object obj)
        {
            if (obj is ParticleEffectAsset asset)
            {
                return Equals(asset);
            }

            return false;
        }

        public static bool operator ==(ParticleEffectAsset left, ParticleEffectAsset right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ParticleEffectAsset left, ParticleEffectAsset right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Converts a <see cref="ParticleEffectAsset"/> to a native input argument.
        /// </summary>
        public static implicit operator InputArgument(ParticleEffectAsset asset)
        {
            return asset.AssetName;
        }

        public override int GetHashCode()
        {
            return AssetName.GetHashCode();
        }

        public override string ToString()
        {
            return AssetName;
        }
    }
}
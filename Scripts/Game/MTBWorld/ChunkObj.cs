using System;
using UnityEngine;
namespace MTB
{
    public class ChunkObj : MonoBehaviour, IDisposable
    {
        #region IDisposable implementation

        public void Dispose()
        {
            terrainMesh.Dispose();
            supportColliderMesh.Dispose();
            terrainMesh = null;
            supportColliderMesh = null;
            chunk = null;
            GameObject.Destroy(gameObject);
        }

        #endregion

        public ChunkMesh terrainMesh;
        public ChunkMesh supportColliderMesh;

        public Chunk chunk;
        public ChunkObj()
        {
        }

        void Start()
        {
            terrainMesh.SetChunkTexture("_TileTex", WorldTextureProvider.Instance.GetAtlasTexture(WorldTextureType.OpaqueTileTex));
            terrainMesh.SetChunkTexture("_NoTileTex", WorldTextureProvider.Instance.GetAtlasTexture(WorldTextureType.OpaqueNoTileTex));
            supportColliderMesh.SetChunkTexture("_TileTex", WorldTextureProvider.Instance.GetAtlasTexture(WorldTextureType.TransparentTileTex));
            supportColliderMesh.SetChunkTexture("_NoTileTex", WorldTextureProvider.Instance.GetAtlasTexture(WorldTextureType.TransparentNoTileTex));

            terrainMesh.SetTexWidth("_TileTexWidth", WorldTextureProvider.Instance.GetTileWidth(WorldTextureType.OpaqueTileTex));
            terrainMesh.SetTexWidth("_TileTexHeight", WorldTextureProvider.Instance.GetTileHeight(WorldTextureType.OpaqueTileTex));
            supportColliderMesh.SetTexWidth("_TileTexWidth", WorldTextureProvider.Instance.GetTileWidth(WorldTextureType.TransparentTileTex));
            supportColliderMesh.SetTexWidth("_TileTexHeight", WorldTextureProvider.Instance.GetTileHeight(WorldTextureType.TransparentTileTex));

			supportColliderMesh.SetTexWidth("_NoTileTexWidth", WorldTextureProvider.Instance.GetTileWidth(WorldTextureType.TransparentNoTileTex));
			supportColliderMesh.SetTexWidth("_NoTileTexHeight", WorldTextureProvider.Instance.GetTileHeight(WorldTextureType.TransparentNoTileTex));

			Rect uvRect = WorldTextureProvider.Instance.GetBlockTextureRect(BlockType.FlowingWater,0,Direction.up);
			supportColliderMesh.SetTexWidth("_FlowingWaterStartX", uvRect.x);
			supportColliderMesh.SetTexWidth("_FlowingWaterStartY", uvRect.y);
		}
    }
}


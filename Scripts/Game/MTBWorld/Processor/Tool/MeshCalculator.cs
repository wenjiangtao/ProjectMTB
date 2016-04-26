using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class MeshCalculator
	{
		private int[] dims;

		private bool[] _faceMask;
		private MeshColliderType[] _colliderMask;
		private Block[] _prevBlocks;

		//是否需要渲染
		private bool[] _renderMask;
		//渲染方向
		private Direction[] _renderDirectionMask;
		//当前渲染网格
		private Block[] _curRenderBlocks;

		private LightVertice[] _sunLightsMask;
		private LightVertice[] _blockLightsMask;

		private MeshColliderType[] _terrainColliderMask;


		public MeshCalculator ()
		{
			dims = new int[]{Chunk.chunkWidth,Section.sectionHeight,Chunk.chunkDepth};

			_faceMask = new bool[256];
			_colliderMask = new MeshColliderType[256];
			_prevBlocks = new Block[256];
			_renderMask = new bool[256];
			_renderDirectionMask = new Direction[256];
			_curRenderBlocks = new Block[256];
			_sunLightsMask = new LightVertice[256];
			_blockLightsMask = new LightVertice[256];

			_terrainColliderMask = new MeshColliderType[dims[0] * dims[1] * dims[2]];
		}

		private List<Section> aroundSectins = new List<Section>();
		private MeshCalculateWay GetCalculateWay(Section section)
		{
			SectionVisible visible = section.AllCanVisible();
			if(visible == SectionVisible.SomeVisible)return MeshCalculateWay.All;
			aroundSectins.Clear();
			List<Section> list = section.chunk.GetAroundSection(section,aroundSectins);
			int sectionIndex;
			for(sectionIndex = 0; sectionIndex < list.Count; sectionIndex++) {
				SectionVisible tempVisible = list[sectionIndex].AllCanVisible();
				if(tempVisible != visible)
				{
					break;
				}
			}
			if(sectionIndex < list.Count)
			{
				return MeshCalculateWay.Side;
			}
			return MeshCalculateWay.None;

		}

		private int[] x = new int[3];
		private int[] q = new int[3];
		private int[] du = new int[3];
		private int[] dv = new int[3];

		private void ClearIntArr(int[] arr)
		{
			for (int i = 0; i < arr.Length; i++) {
				arr[i] = 0;
			}
		}

		public void CalculateSectionMesh(Section section,MeshData meshData)
		{
			MeshCalculateWay way = GetCalculateWay(section);
			if(way == MeshCalculateWay.None)return;
			for (int d = 0; d < 3; d++) {
				int i,j,k,l,w,h,
				u = (d + 1) % 3,
				v = (d + 2) % 3;
				ClearIntArr(x);
				ClearIntArr(q);
//				int[] x = new int[]{0,0,0};
//				int[] q = new int[]{0,0,0};
				
				int direction = d + 1;
				
				q[d] = 1;
				if(way == MeshCalculateWay.Side)
				{
					x[d] = dims[d] - 1;
				}
				else
				{
					x[d] = 0;
				}
				for (; x[d] < dims[d];) {
					int n = 0;
					bool bBInRange = x[d] < dims[d] - 1;
					for (x[v] = 0; x[v] < dims[v]; x[v]++) {
						for (x[u] = 0; x[u] < dims[u]; x[u]++) {
							int x1 = x[0],y1 = x[1],z1 = x[2],x2 = x1 + q[0],y2 = y1 + q[1],z2 = z1 + q[2];

							Block bA = (x[d] < 1 || way == MeshCalculateWay.Side) ? section.GetBlock(x1,y1,z1,true) : _prevBlocks[n];
							BlockAttributeCalculator aBAC = BlockAttributeCalculatorFactory.GetCalculator(bA.BlockType);

							Block bB = section.GetBlock(x2,y2,z2,bBInRange);
							BlockAttributeCalculator bBAC = BlockAttributeCalculatorFactory.GetCalculator(bB.BlockType);

							CalculateRenderMesh(section.chunk,meshData,x1,y1 + section.chunkOffsetY,z1,x2,y2 + section.chunkOffsetY,z2,bA,bB,aBAC,bBAC,direction,bBInRange);
							RefreshRenderMask(_renderMask,_renderDirectionMask,_curRenderBlocks,_sunLightsMask,_blockLightsMask,n,section,x1,y1,z1,x2,y2,z2,bA,bB,aBAC,bBAC,bBInRange,direction);
							RefreshColliderMask(_faceMask,_colliderMask,n,bA,bB,aBAC,bBAC,direction);

							_prevBlocks[n] = bB;
							n++;
						}
					}
					x[d]++;

//					//贪心简化碰撞网格
//					n = 0;
//					for (j = 0; j < dims[v]; j++) {
//						for (i = 0; i < dims[u];) {
//							bool c = _faceMask[n];
//							MeshColliderType c1 = _colliderMask[n];
//							if(c1 != MeshColliderType.none)
//							{
//								for (w = 1;i + w < dims[u] && c1 == _colliderMask[n + w] && c == _faceMask[n + w] ; w++);
//								bool done = false;
//								for (h = 1; j + h < dims[v]; h++) {
//									for (k = 0; k < w; k++) {
//										int doneIndex = n + k + h * dims[u];
//										if(c1 != _colliderMask[doneIndex] || c != _faceMask[doneIndex])
//										{
//											done = true;
//											break;
//										}
//									}
//									if(done)break;
//								}
//								
//								x[u] = i; x[v] = j;
//								ClearIntArr(du);
//								ClearIntArr(dv);
////								int[] du = new int[]{0,0,0};
////								int[] dv = new int[]{0,0,0};
//								du[u] = w;
//								dv[v] = h;
//								//因为shader的需求，这边不能-0.5
//								AddFace(c,c1,meshData,MeshBaseDataCache.Instance.GetVector3(x[0],x[1] + section.chunkOffsetY,x[2]),
//								        MeshBaseDataCache.Instance.GetVector3(x[0] + du[0],x[1] + du[1] + section.chunkOffsetY,x[2] + du[2]),
//								        MeshBaseDataCache.Instance.GetVector3(x[0] + du[0] + dv[0],x[1] + du[1] + dv[1] + section.chunkOffsetY,x[2] + du[2] + dv[2]),
//								        MeshBaseDataCache.Instance.GetVector3(x[0] + dv[0],x[1] + dv[1] + section.chunkOffsetY,x[2] + dv[2]));
//								for (l = 0; l < h; l++) {
//									for (k = 0; k < w; k++) {
//										int maskIndex = n + k + l * dims[u];
//										_faceMask[maskIndex] = true;
//										_colliderMask[maskIndex] = MeshColliderType.none;
//									}
//								}
//								i+=w;
//								n+=w;
//							}
//							else
//							{
//								i++;n++;
//							}
//						}
//					}

					//贪心简化渲染网格

					n = 0;
					for (j = 0; j < dims[v]; j++) {
						for (i = 0; i < dims[u];) {
							bool canRender = _renderMask[n];
							Direction renderDirection = _renderDirectionMask[n];
							Block curBlock = _curRenderBlocks[n];
							BlockAttributeCalculator curCalculator = BlockAttributeCalculatorFactory.GetCalculator(curBlock.BlockType);
							LightVertice sunLight = _sunLightsMask[n];
							LightVertice blockLight = _blockLightsMask[n];
							if(canRender)
							{
								for (w = 1;i + w < dims[u] && _renderMask[n + w] && renderDirection == _renderDirectionMask[n + w] &&
								     curCalculator.CanCombineWithBlock(curBlock.ExtendId,_curRenderBlocks[n + w]) &&
								     sunLight.EqualOther(_sunLightsMask[n + w]) && blockLight.EqualOther(_blockLightsMask[n + w]); w++);
								bool done = false;
								for (h = 1; j + h < dims[v]; h++) {
									for (k = 0; k < w; k++) {
										int doneIndex = n + k + h * dims[u];
										if(!_renderMask[doneIndex] || renderDirection != _renderDirectionMask[doneIndex]
										   || !curCalculator.CanCombineWithBlock(curBlock.ExtendId,_curRenderBlocks[doneIndex])
										   || !sunLight.EqualOther(_sunLightsMask[doneIndex]) || !blockLight.EqualOther(_blockLightsMask[doneIndex]))
										{
											done = true;
											break;
										}
									}
									if(done)break;
								}
								
								x[u] = i; x[v] = j;
								ClearIntArr(du);
								ClearIntArr(dv);
//								int[] du = new int[]{0,0,0};
//								int[] dv = new int[]{0,0,0};
								du[u] = w;
								dv[v] = h;
								curCalculator.CalculateMesh(section.chunk,meshData,curBlock,renderDirection,
								                            MeshBaseDataCache.Instance.GetVector3(x[0],x[1] + section.chunkOffsetY,x[2]),
								                            MeshBaseDataCache.Instance.GetVector3(x[0] + du[0],x[1] + du[1] + section.chunkOffsetY,x[2] + du[2]),
								                            MeshBaseDataCache.Instance.GetVector3(x[0] + du[0] + dv[0],x[1] + du[1] + dv[1] + section.chunkOffsetY,x[2] + du[2] + dv[2]),
								                            MeshBaseDataCache.Instance.GetVector3(x[0] + dv[0],x[1] + dv[1] + section.chunkOffsetY,x[2] + dv[2]),sunLight,blockLight);
								for (l = 0; l < h; l++) {
									for (k = 0; k < w; k++) {
										int maskIndex = n + k + l * dims[u];
										_renderMask[maskIndex] = false;
									}
								}
								i+=w;
								n+=w;
							}
							else
							{
								i++;n++;
							}
						}
					}
				}
			}
			//计算boxcollider
			int nn = 0;
			for (int y = 0; y < dims[1]; y++) {
				for (int x = 0; x < dims[0]; x++) {
					for (int z = 0; z < dims[2]; z++) {
						Block block = section.GetBlock(x,y,z,true);
						BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(block.BlockType);
						MeshColliderType colliderType = calculator.GetMeshColliderType(block.ExtendId);
						_terrainColliderMask[nn] = colliderType;
						nn++;
					}
				}
			}
			nn = 0;
			int dd;
			int ww;
			int hh;
			int kk;
			int ll;
			int mm;
			for (int y = 0; y < dims[1]; y++) {
				for (int x = 0; x < dims[0]; x++) {
					for (int z = 0; z < dims[2];) {
						MeshColliderType flag = _terrainColliderMask[nn];
						if(flag != MeshColliderType.none)
						{
							for (dd = 1;z + dd < dims[2] && flag == _terrainColliderMask[nn + dd] ; dd++);
							bool done = false;
							for (ww = 1; x + ww < dims[0]; ww++) {
								for (kk = 0; kk < dd; kk++) {
									int doneIndex = nn + kk + ww * dims[2];
									if(flag != _terrainColliderMask[doneIndex])
									{
										done = true;
										break;
									}
								}
								if(done)break;
							}
							done = false;
							for (hh = 1; y + hh < dims[1]; hh++) {
								for (ll = 0; ll < ww; ll++) {
									for (kk = 0; kk < dd; kk++) {
										int doneIndex = nn + kk + ll * dims[2] + hh * dims[0] * dims[2];
										if(flag != _terrainColliderMask[doneIndex])
										{
											done = true;
											break;
										}
									}
									if(done)break;
								}
								if(done)break;
							}
							
							Vector3 min = new Vector3(x,y,z);
							Vector3 max = new Vector3(x + ww,y + hh,z + dd);
							Vector3 center = (min+ max) / 2;
							center.y += section.chunkOffsetY;
							Vector3 size = max - min;
							if(flag == MeshColliderType.terrainCollider)
							{
								meshData.terrainCollider.AddBox(center,size);
							}
							else
							{
								meshData.supportCollider.AddBox(center,size);
							}

							for (mm = 0; mm < hh; mm++) {
								for (ll = 0; ll < ww; ll++) {
									for (kk = 0; kk < dd; kk++) {
										int maskIndex = nn + kk + ll * dims[2] + mm * dims[0] * dims[2];
										_terrainColliderMask[maskIndex] = MeshColliderType.none;
									}
								}
							}
							nn += dd;
							z += dd;
						}
						else
						{
							nn++;
							z++;
						}
					}
				}
			}
		}

		private void CalculateRenderMesh(Chunk chunk,MeshData meshData,int x1,int y1,int z1,int x2,int y2,int z2,
		                                 Block bA,Block bB,BlockAttributeCalculator aBAC,BlockAttributeCalculator bBAC,int direction,bool bBInRange)
		{
			BlockRenderType aBACRenderType = aBAC.GetBlockRenderType(bA.ExtendId);
			if(aBACRenderType == BlockRenderType.Part)
			{
				//渲染（特殊）物块
				aBAC.CalculateSpecialMesh(chunk,x1,y1,z1,meshData,bA,bB,bBAC,(Direction)direction);
			}

			BlockRenderType bBACRenderType = bBAC.GetBlockRenderType(bB.ExtendId);
			if(bBACRenderType == BlockRenderType.Part)
			{
				//不能因为x2,y2,z2有可能不在chunk中 而需要保证x2,y2,z2在chunk中)
//				Chunk tempChunk = chunk;
//				if(!bBInRange)
//				{
//					int x2WorldPos = chunk.worldPos.x + x2;
//					int y2WorldPos = chunk.worldPos.y + y2;
//					int z2WorldPos = chunk.worldPos.z + z2;
//					tempChunk = chunk.world.GetChunk(x2WorldPos,y2WorldPos,z2WorldPos);
//					x2 = x2WorldPos - tempChunk.worldPos.x;
//					y2 = y2WorldPos - tempChunk.worldPos.y;
//					z2 = z2WorldPos - tempChunk.worldPos.z;
//				}
				//渲染（特殊）物块（不能确定当前坐标会在chunk中）
				bBAC.CalculateSpecialMesh(chunk,x2,y2,z2,meshData,bB,bA,aBAC,(Direction)(-direction));
			}
		}

		private void RefreshRenderMask(bool[] renderMask,Direction[] renderDirectionMask,Block[] curRenderBlocks,LightVertice[] sunLightsMask,LightVertice[] blockLightsMask,
		                               int maskIndex,Section section,int x1,int y1,int z1,int x2,int y2,int z2,Block bA,Block bB,BlockAttributeCalculator aBAC,
		                               BlockAttributeCalculator bBAC,bool bBInRange,int direction)
		{
			if(aBAC.CanCalculateMesh(bA,bB,bBAC,(Direction)(direction)))
			{
				renderMask[maskIndex] = true;
				renderDirectionMask[maskIndex] = (Direction)(direction);
				curRenderBlocks[maskIndex] = bA;
				sunLightsMask[maskIndex] = GetSunLightVertice(section,x1,y1,z1,(Direction)(direction));
				blockLightsMask[maskIndex] = GetBlockLightVertice(section,x1,y1,z1,(Direction)(direction));
//				sunLightsMask[maskIndex] = section.GetSunLight(x2,y2,z2,bBInRange);
//				blockLightsMask[maskIndex] = section.GetBlockLight(x2,y2,z2,bBInRange);
			}
			else if(bBAC.CanCalculateMesh(bB,bA,aBAC,(Direction)(-direction)))
			{
				renderMask[maskIndex] = true;
				renderDirectionMask[maskIndex] = (Direction)(-direction);
				curRenderBlocks[maskIndex] = bB;
//				sunLightsMask[maskIndex] = section.GetSunLight(x1,y1,z1,true);
//				blockLightsMask[maskIndex] = section.GetBlockLight(x1,y1,z1,true);
				sunLightsMask[maskIndex] = GetSunLightVertice(section,x2,y2,z2,(Direction)(-direction));
				blockLightsMask[maskIndex] = GetBlockLightVertice(section,x2,y2,z2,(Direction)(-direction));

			}
			else
			{
				renderMask[maskIndex] = false;
			}
		}

		private void RefreshColliderMask(bool[] faceMask,MeshColliderType[] colliderMask,int maskIndex,Block bA,Block bB,BlockAttributeCalculator aBAC,BlockAttributeCalculator bBAC,int direction)
		{
			MeshColliderType a = aBAC.GetMeshColliderType(bA.ExtendId);
			MeshColliderType b = bBAC.GetMeshColliderType(bB.ExtendId);
			if(a == b)
			{
				faceMask[maskIndex] = true;
				colliderMask[maskIndex] = MeshColliderType.none;
			}
			else if(b == MeshColliderType.none)
			{
				faceMask[maskIndex] = true;
				colliderMask[maskIndex] = aBAC.GetMeshColliderType(bA.ExtendId);
			}
			else if(a == MeshColliderType.none)
			{
				faceMask[maskIndex] = false;
				colliderMask[maskIndex] = bBAC.GetMeshColliderType(bB.ExtendId);
			}
			else 
			{
				if(a == MeshColliderType.terrainCollider)
				{
					faceMask[maskIndex] = true;
					colliderMask[maskIndex] = aBAC.GetMeshColliderType(bA.ExtendId);
				}
				else
				{
					faceMask[maskIndex] = false;
					colliderMask[maskIndex] = bBAC.GetMeshColliderType(bB.ExtendId);
				}
			}
		}

		private void AddFace(bool faceMask,MeshColliderType meshColliderType,MeshData meshData,Vector3 v1,Vector3 v2,Vector3 v3,Vector3 v4)
		{
			if(meshColliderType == MeshColliderType.terrainCollider)
			{
				meshData.useRenderDataForCol = true;
				meshData.useSupportDataForCol = false;
			}
			else if(meshColliderType == MeshColliderType.supportCollider)
			{
				meshData.useRenderDataForCol = false;
				meshData.useSupportDataForCol = true;
			}
			meshData.AddColVertice(v1);
			meshData.AddColVertice(v2);
			meshData.AddColVertice(v3);
			meshData.AddColVertice(v4);

			meshData.AddColQuadTriangles(faceMask);
			
			meshData.useRenderDataForCol = MeshData.DefaultUseRenderDataForCol;
			meshData.useSupportDataForCol = MeshData.DefaultUseSupportDataForCol;
		}

		//返回点的顺序要与传入点的顺序一致
		public LightVertice GetSunLightVertice(Section section,int x,int y,int z,Direction direction)
		{
			LightVertice lv = new LightVertice();
			switch(direction)
			{
			case Direction.up:
				lv.v00 = GetSunLightAverageValue(section,x,y + 1,z,x - 1,y + 1,z,x - 1,y + 1,z - 1,x,y + 1,z - 1);
				lv.v01 = GetSunLightAverageValue(section,x,y + 1,z,x - 1,y + 1,z,x - 1,y + 1,z + 1,x,y + 1,z + 1);
				lv.v11 = GetSunLightAverageValue(section,x,y + 1,z,x + 1,y + 1,z,x + 1,y + 1,z + 1,x,y + 1,z + 1);
				lv.v10 = GetSunLightAverageValue(section,x,y + 1,z,x + 1,y + 1,z,x + 1,y + 1,z - 1,x,y + 1,z - 1);
				break;
			case Direction.down:
				lv.v00 = GetSunLightAverageValue(section,x,y - 1,z,x - 1,y - 1,z,x - 1,y - 1,z - 1,x,y - 1,z - 1);
				lv.v01 = GetSunLightAverageValue(section,x,y - 1,z,x - 1,y - 1,z,x - 1,y - 1,z + 1,x,y - 1,z + 1);
				lv.v11 = GetSunLightAverageValue(section,x,y - 1,z,x + 1,y - 1,z,x + 1,y - 1,z + 1,x,y - 1,z + 1);
				lv.v10 = GetSunLightAverageValue(section,x,y - 1,z,x + 1,y - 1,z,x + 1,y - 1,z - 1,x,y - 1,z - 1);
				break;
			case Direction.left:
				lv.v00 = GetSunLightAverageValue(section,x - 1,y,z,x - 1,y - 1,z,x - 1,y - 1,z - 1,x - 1,y,z - 1);
				lv.v01 = GetSunLightAverageValue(section,x - 1,y,z,x - 1,y + 1,z,x - 1,y + 1,z - 1,x - 1,y,z - 1);
				lv.v11 = GetSunLightAverageValue(section,x - 1,y,z,x - 1,y + 1,z,x - 1,y + 1,z + 1,x - 1,y,z + 1);
				lv.v10 = GetSunLightAverageValue(section,x - 1,y,z,x - 1,y - 1,z,x - 1,y - 1,z + 1,x - 1,y,z + 1);
				break;
			case Direction.right:
				lv.v00 = GetSunLightAverageValue(section,x + 1,y,z,x + 1,y - 1,z,x + 1,y - 1,z - 1,x + 1,y,z - 1);
				lv.v01 = GetSunLightAverageValue(section,x + 1,y,z,x + 1,y + 1,z,x + 1,y + 1,z - 1,x + 1,y,z - 1);
				lv.v11 = GetSunLightAverageValue(section,x + 1,y,z,x + 1,y + 1,z,x + 1,y + 1,z + 1,x + 1,y,z + 1);
				lv.v10 = GetSunLightAverageValue(section,x + 1,y,z,x + 1,y - 1,z,x + 1,y - 1,z + 1,x + 1,y,z + 1);
				break;
			case Direction.front:
				lv.v00 = GetSunLightAverageValue(section,x,y,z + 1,x - 1,y,z + 1,x - 1,y - 1,z + 1,x,y - 1,z + 1);
				lv.v01 = GetSunLightAverageValue(section,x,y,z + 1,x + 1,y,z + 1,x + 1,y - 1,z + 1,x,y - 1,z + 1);
				lv.v11 = GetSunLightAverageValue(section,x,y,z + 1,x + 1,y,z + 1,x + 1,y + 1,z + 1,x,y + 1,z + 1);
				lv.v10 = GetSunLightAverageValue(section,x,y,z + 1,x - 1,y,z + 1,x - 1,y + 1,z + 1,x,y + 1,z + 1);
				break;
			case Direction.back:
				lv.v00 = GetSunLightAverageValue(section,x,y,z - 1,x - 1,y,z - 1,x - 1,y - 1,z - 1,x,y - 1,z - 1);
				lv.v01 = GetSunLightAverageValue(section,x,y,z - 1,x + 1,y,z - 1,x + 1,y - 1,z - 1,x,y - 1,z - 1);
				lv.v11 = GetSunLightAverageValue(section,x,y,z - 1,x + 1,y,z - 1,x + 1,y + 1,z - 1,x,y + 1,z - 1);
				lv.v10 = GetSunLightAverageValue(section,x,y,z - 1,x - 1,y,z - 1,x - 1,y + 1,z - 1,x,y + 1,z - 1);
				break;
			}
			return lv;
		}

		//返回点的顺序要与传入点的顺序一致
		public LightVertice GetBlockLightVertice(Section section,int x,int y,int z,Direction direction)
		{
			LightVertice lv = new LightVertice();
			switch(direction)
			{
			case Direction.up:
				lv.v00 = GetBlockLightAverageValue(section,x,y + 1,z,x - 1,y + 1,z,x - 1,y + 1,z - 1,x,y + 1,z - 1);
				lv.v01 = GetBlockLightAverageValue(section,x,y + 1,z,x - 1,y + 1,z,x - 1,y + 1,z + 1,x,y + 1,z + 1);
				lv.v11 = GetBlockLightAverageValue(section,x,y + 1,z,x + 1,y + 1,z,x + 1,y + 1,z + 1,x,y + 1,z + 1);
				lv.v10 = GetBlockLightAverageValue(section,x,y + 1,z,x + 1,y + 1,z,x + 1,y + 1,z - 1,x,y + 1,z - 1);
				break;
			case Direction.down:
				lv.v00 = GetBlockLightAverageValue(section,x,y - 1,z,x - 1,y - 1,z,x - 1,y - 1,z - 1,x,y - 1,z - 1);
				lv.v01 = GetBlockLightAverageValue(section,x,y - 1,z,x - 1,y - 1,z,x - 1,y - 1,z + 1,x,y - 1,z + 1);
				lv.v11 = GetBlockLightAverageValue(section,x,y - 1,z,x + 1,y - 1,z,x + 1,y - 1,z + 1,x,y - 1,z + 1);
				lv.v10 = GetBlockLightAverageValue(section,x,y - 1,z,x + 1,y - 1,z,x + 1,y - 1,z - 1,x,y - 1,z - 1);
				break;
			case Direction.left:
				lv.v00 = GetBlockLightAverageValue(section,x - 1,y,z,x - 1,y - 1,z,x - 1,y - 1,z - 1,x - 1,y,z - 1);
				lv.v01 = GetBlockLightAverageValue(section,x - 1,y,z,x - 1,y + 1,z,x - 1,y + 1,z - 1,x - 1,y,z - 1);
				lv.v11 = GetBlockLightAverageValue(section,x - 1,y,z,x - 1,y + 1,z,x - 1,y + 1,z + 1,x - 1,y,z + 1);
				lv.v10 = GetBlockLightAverageValue(section,x - 1,y,z,x - 1,y - 1,z,x - 1,y - 1,z + 1,x - 1,y,z + 1);
				break;
			case Direction.right:
				lv.v00 = GetBlockLightAverageValue(section,x + 1,y,z,x + 1,y - 1,z,x + 1,y - 1,z - 1,x + 1,y,z - 1);
				lv.v01 = GetBlockLightAverageValue(section,x + 1,y,z,x + 1,y + 1,z,x + 1,y + 1,z - 1,x + 1,y,z - 1);
				lv.v11 = GetBlockLightAverageValue(section,x + 1,y,z,x + 1,y + 1,z,x + 1,y + 1,z + 1,x + 1,y,z + 1);
				lv.v10 = GetBlockLightAverageValue(section,x + 1,y,z,x + 1,y - 1,z,x + 1,y - 1,z + 1,x + 1,y,z + 1);
				break;
			case Direction.front:
				lv.v00 = GetBlockLightAverageValue(section,x,y,z + 1,x - 1,y,z + 1,x - 1,y - 1,z + 1,x,y - 1,z + 1);
				lv.v01 = GetBlockLightAverageValue(section,x,y,z + 1,x + 1,y,z + 1,x + 1,y - 1,z + 1,x,y - 1,z + 1);
				lv.v11 = GetBlockLightAverageValue(section,x,y,z + 1,x + 1,y,z + 1,x + 1,y + 1,z + 1,x,y + 1,z + 1);
				lv.v10 = GetBlockLightAverageValue(section,x,y,z + 1,x - 1,y,z + 1,x - 1,y + 1,z + 1,x,y + 1,z + 1);
				break;
			case Direction.back:
				lv.v00 = GetBlockLightAverageValue(section,x,y,z - 1,x - 1,y,z - 1,x - 1,y - 1,z - 1,x,y - 1,z - 1);
				lv.v01 = GetBlockLightAverageValue(section,x,y,z - 1,x + 1,y,z - 1,x + 1,y - 1,z - 1,x,y - 1,z - 1);
				lv.v11 = GetBlockLightAverageValue(section,x,y,z - 1,x + 1,y,z - 1,x + 1,y + 1,z - 1,x,y + 1,z - 1);
				lv.v10 = GetBlockLightAverageValue(section,x,y,z - 1,x - 1,y,z - 1,x - 1,y + 1,z - 1,x,y + 1,z - 1);
				break;
			}
			return lv;
		}

		private int[] sunColorArr = new int[4];
		private float GetSunLightAverageValue(Section section,int x1,int y1,int z1,int x2,int y2,int z2,int x3,int y3,int z3,int x4,int y4,int z4)
		{
			int a = section.GetSunLight(x1,y1,z1);
			int b = section.GetSunLight(x2,y2,z2);
			int c = section.GetSunLight(x3,y3,z3);
			int d = section.GetSunLight(x4,y4,z4);
			sunColorArr[0] = a;
			sunColorArr[1] = b;
			sunColorArr[2] = c;
			sunColorArr[3] = d;
			return GetAverageColor(sunColorArr);
		}

		private int[] blockColorArr = new int[4];
		private float GetBlockLightAverageValue(Section section,int x1,int y1,int z1,int x2,int y2,int z2,int x3,int y3,int z3,int x4,int y4,int z4)
		{
			int a = section.GetBlockLight(x1,y1,z1);
			int b = section.GetBlockLight(x2,y2,z2);
			int c = section.GetBlockLight(x3,y3,z3);
			int d = section.GetBlockLight(x4,y4,z4);
			blockColorArr[0] = a;
			blockColorArr[1] = b;
			blockColorArr[2] = c;
			blockColorArr[3] = d;
			return GetAverageColor(blockColorArr);
		}

		private float GetAverageColor(int[] color)
		{
			int index = 0;
			float result = 0;
			for (int i = 0; i < color.Length; i++) {
				if(color[i] != 0)
				{
					index++;
					result += LightConst.lightColor[color[i]];
				}
			}
			if(index == 0)return LightConst.lightColor[0];
			float r = result / index;
			//将有棱角的地方亮度减少
			r -=(4 - index) * WorldConfig.Instance.lightEdgeReduce;
			return r < LightConst.lightColor[0] ? LightConst.lightColor[0] : r;
		}
	}

	public enum MeshCalculateWay
	{
		None,
		All,
		Side
	}
}


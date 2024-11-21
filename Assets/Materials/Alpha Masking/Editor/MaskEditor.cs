using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ToJ
{
	[CustomEditor(typeof(Mask))]
	public class MaskEditor : Editor
	{

		public override void OnInspectorGUI()
		{
			Mask maskTarget = (Mask)target;

			if (maskTarget.GetComponents<Mask>().Length > 1)
			{
				GUILayout.Label("More than one instance of Mask attached.\nPlease only use one.");
				return;
			}

			if ((maskTarget.GetComponent<MeshRenderer>() != null) &&
				(maskTarget.GetComponent<MeshFilter>() != null) &&
				(maskTarget.GetComponent<Renderer>().sharedMaterial != null) &&
				(maskTarget.GetComponent<Renderer>().sharedMaterial.mainTexture != null))
			{

				//maskTarget.maskMappingWorldAxis = (Mask.MappingAxis)EditorGUILayout.EnumPopup("Mask Mapping World Axis", maskTarget.maskMappingWorldAxis);

				Mask.MappingAxis maskMappingWorldAxis = (Mask.MappingAxis)EditorGUILayout.EnumPopup("Mask Mapping World Axis", maskTarget.maskMappingWorldAxis);
				if (maskMappingWorldAxis != maskTarget.maskMappingWorldAxis)
				{
					maskTarget.maskMappingWorldAxis = maskMappingWorldAxis;
				}


				bool invertAxis = EditorGUILayout.Toggle("Invert Axis", maskTarget.invertAxis);
				if (invertAxis != maskTarget.invertAxis)
				{
					maskTarget.invertAxis = invertAxis;
				}

				bool clampAlphaHorizontally = EditorGUILayout.Toggle("Clamp Alpha Horizontally", maskTarget.clampAlphaHorizontally);
				if (clampAlphaHorizontally != maskTarget.clampAlphaHorizontally)
				{
					maskTarget.clampAlphaHorizontally = clampAlphaHorizontally;
				}

				bool clampAlphaVertically = EditorGUILayout.Toggle("Clamp Alpha Vertically", maskTarget.clampAlphaVertically);
				if (clampAlphaVertically != maskTarget.clampAlphaVertically)
				{
					maskTarget.clampAlphaVertically = clampAlphaVertically;
				}

				float clampingBorder = EditorGUILayout.FloatField("Clamping Border", maskTarget.clampingBorder);
				if (clampingBorder != maskTarget.clampingBorder)
				{
					maskTarget.clampingBorder = clampingBorder;
				}

				bool useMaskAlphaChannel = EditorGUILayout.Toggle("Use Mask Alpha Channel (not RGB)", maskTarget.useMaskAlphaChannel);
				if (useMaskAlphaChannel != maskTarget.useMaskAlphaChannel)
				{
					maskTarget.useMaskAlphaChannel = useMaskAlphaChannel;
				}


				if (!Application.isPlaying)
				{
					bool displayMask = EditorGUILayout.Toggle("Display Mask", maskTarget.GetComponent<Renderer>().enabled);
					if (displayMask != maskTarget.GetComponent<Renderer>().enabled)
					{
						maskTarget.GetComponent<Renderer>().enabled = displayMask;
					}

				}

				if (!Application.isPlaying)
				{
					if (GUILayout.Button("Apply Mask to Siblings in Hierarchy"))
					{
						ApplyMaskToChildren();
					}
				}

			}
			else
			{
				GUILayout.Label("Please attach MeshFilter and MeshRenderer.\nAlso assign a texture to MeshRenderer.");
			}

			if (GUI.changed)
			{
				EditorUtility.SetDirty(target);
			}
		}


		private void ApplyMaskToChildren()
		{
			Mask maskTarget = (Mask)target;
			Shader maskedSpriteWorldCoordsShader = Shader.Find("Alpha Masked/Sprites Alpha Masked - World Coords");
			Shader maskedUnlitWorldCoordsShader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
			Shader spriteDefaultShader = Shader.Find("Sprites/Default");
			Shader unlitTransparentShader = Shader.Find("Unlit/Transparent");
			Shader UIDefaultShader = Shader.Find("UI/Default");
			Shader UIDefaultFontShader = Shader.Find("UI/Default Font");

			if ((maskedSpriteWorldCoordsShader == null) || (maskedUnlitWorldCoordsShader == null))
			{
				Debug.Log("Shaders necessary for masking don't seem to be present in the project.");
				return;
			}

			Texture maskTexture = maskTarget.GetComponent<Renderer>().sharedMaterial.mainTexture;
		
			List<Component> components = new List<Component>();
			components.AddRange(maskTarget.transform.parent.gameObject.GetComponentsInChildren<Renderer>());
			components.AddRange(maskTarget.transform.parent.gameObject.GetComponentsInChildren<Graphic>());
			List<Material> differentOriginalMaterials = new List<Material>();
			List<Material> differentNewMaterials = new List<Material>();

			List<Material> differentReusableMaterials = GetAllReusableMaterials(components, maskedSpriteWorldCoordsShader, maskedUnlitWorldCoordsShader);// new List<Material>();

			foreach (Component component in components)
			{
				if (component.gameObject != maskTarget.gameObject)
				{
					List<Material> currSharedMaterials = new List<Material>();
					List<bool> areCurrSharedMaterialsTexts = new List<bool>();
					if (component is Renderer)
					{
						currSharedMaterials.AddRange(((Renderer)component).sharedMaterials);
						for (int i = 0; i < ((Renderer)component).sharedMaterials.Length; i++)
						{
							areCurrSharedMaterialsTexts.Add(false);
						}
					}
					else if (component is Graphic)
					{
						currSharedMaterials.Add(((Graphic)component).material);
						areCurrSharedMaterialsTexts.Add(component is Text);
					}

					bool materialsChanged = false;

					for (int i = 0; i < currSharedMaterials.Count; i++)
					{
						Material material = currSharedMaterials[i];

						if (!differentOriginalMaterials.Contains(material))
						{
							Material materialToBeUsed = null;

							if ((material.shader == spriteDefaultShader) ||
								(material.shader == unlitTransparentShader) ||
								(material.shader == UIDefaultShader) ||
								(material.shader == UIDefaultFontShader))
							{
								Material reusableMaterial = FindSuitableMaskedMaterial(material, differentReusableMaterials,
																					   maskedSpriteWorldCoordsShader, maskedUnlitWorldCoordsShader,
																					   spriteDefaultShader, unlitTransparentShader, UIDefaultShader, UIDefaultFontShader);

								if (reusableMaterial == null)
								{
									differentOriginalMaterials.Add(material);

									Material newMaterial = new Material(material);
									if (material.shader == spriteDefaultShader)
									{
										newMaterial.shader = maskedSpriteWorldCoordsShader;
									}
									else if (material.shader == unlitTransparentShader)
									{
										newMaterial.shader = maskedUnlitWorldCoordsShader;
									}
									else if (material.shader == UIDefaultShader)
									{
										newMaterial.shader = maskedSpriteWorldCoordsShader;
									}
									else if (material.shader == UIDefaultFontShader)
									{
										newMaterial.shader = maskedSpriteWorldCoordsShader;
									}
									if (areCurrSharedMaterialsTexts[i] == true)
									{
										newMaterial.SetFloat("_IsThisText", 1);
									}
									newMaterial.name = material.name + " Masked";
									newMaterial.SetTexture("_AlphaTex", maskTexture);

									materialToBeUsed = newMaterial;
									differentNewMaterials.Add(newMaterial);
									currSharedMaterials[i] = newMaterial;
									materialsChanged = true;
								}
								else
								{
									currSharedMaterials[i] = reusableMaterial;
									materialsChanged = true;

									reusableMaterial.SetTexture("_AlphaTex", maskTexture);
									materialToBeUsed = reusableMaterial;
								}
							}
							else if ((material.shader == maskedSpriteWorldCoordsShader) ||
									 (material.shader == maskedUnlitWorldCoordsShader))
							{
								if (material.GetTexture("_AlphaTex") != maskTexture)
								{
									material.SetTexture("_AlphaTex", maskTexture);
								}
								if (areCurrSharedMaterialsTexts[i] == true)
								{
									material.SetFloat("_IsThisText", 1);
								}
								materialToBeUsed = material;
							}

							if (materialToBeUsed != null)
							{
								maskTarget.SetMaskMappingAxisInMaterial(maskTarget.maskMappingWorldAxis, materialToBeUsed);
								maskTarget.SetMaskBoolValueInMaterial("_ClampHoriz", maskTarget.clampAlphaHorizontally, materialToBeUsed);
								maskTarget.SetMaskBoolValueInMaterial("_ClampVert", maskTarget.clampAlphaVertically, materialToBeUsed);
								maskTarget.SetMaskBoolValueInMaterial("_UseAlphaChannel", maskTarget.useMaskAlphaChannel, materialToBeUsed);
							}
						}
						else
						{
							int index = differentOriginalMaterials.IndexOf(material);

							currSharedMaterials[i] = differentNewMaterials[index];
							materialsChanged = true;
						}
					}

					if (materialsChanged == true)
					{
						if (component is Renderer)
						{
							((Renderer)component).sharedMaterials = currSharedMaterials.ToArray();
						}
						else if (component is Graphic)
						{
							((Graphic)component).material = currSharedMaterials[0];
						}
					}

				}
			}

			Debug.Log("Mask applied.");
		}

		private Material FindSuitableMaskedMaterial(Material nonMaskedMaterial, List<Material> differentReusableMaterials,
													 Shader maskedSpriteWorldCoordsShader, Shader maskedUnlitWorldCoordsShader,
													 Shader spriteDefaultShader, Shader unlitTransparentShader, Shader UIDefaultShader, Shader UIDefaultFontShader)
		{
			foreach (Material material in differentReusableMaterials)
			{
				if ((nonMaskedMaterial.shader == spriteDefaultShader) &&
					(material.shader == maskedSpriteWorldCoordsShader))
				{
					if ((material.name == nonMaskedMaterial.name + " Masked") &&
						(material.GetColor("_Color") == nonMaskedMaterial.GetColor("_Color")) &&
						(material.GetFloat("PixelSnap") == nonMaskedMaterial.GetFloat("PixelSnap")))
					{
						return material;
					}
				}
				else if ((nonMaskedMaterial.shader == unlitTransparentShader) &&
						 (material.shader == maskedUnlitWorldCoordsShader))
				{
					if ((material.name == nonMaskedMaterial.name + " Masked") &&
						(material.mainTexture == nonMaskedMaterial.mainTexture))
					{
						return material;
					}
				}
				else if ((nonMaskedMaterial.shader == UIDefaultFontShader) &&
						 (material.shader == maskedSpriteWorldCoordsShader))
				{
					if ((material.name == nonMaskedMaterial.name + " Masked") &&
						(material.GetColor("_Color") == nonMaskedMaterial.GetColor("_Color")))
					{
						return material;
					}
				}
			}

			return null;
		}

		private List<Material> GetAllReusableMaterials(List<Component> components, Shader maskedSpriteWorldCoordsShader, Shader maskedUnlitWorldCoordsShader)
		{
			List<Material> differentReusableMaterials = new List<Material>();
			Mask maskTarget = (Mask)target;

			foreach (Component component in components)
			{
				if (component.gameObject != maskTarget.gameObject)
				{
					List<Material> currSharedMaterials = new List<Material>();
					if (component is Renderer)
					{
						currSharedMaterials.AddRange(((Renderer)component).sharedMaterials);
					}
					else if (component is Graphic)
					{
						currSharedMaterials.Add(((Graphic)component).material);
					}
					//Material[] currSharedMaterials = renderer.sharedMaterials;

					for (int i = 0; i < currSharedMaterials.Count; i++)
					{
						Material material = currSharedMaterials[i];

						if ((material.shader == maskedSpriteWorldCoordsShader) ||
							(material.shader == maskedUnlitWorldCoordsShader))
						{
							differentReusableMaterials.Add(material);
						}
					}
				}
			}

			return differentReusableMaterials;
		}
	}
}
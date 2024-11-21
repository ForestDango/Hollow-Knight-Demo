using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class FixVerticalAlign : MonoBehaviour
{
    private Text text;
    public LabelFixType labelFixType;


    private void OnEnable()
    {
	if (labelFixType == LabelFixType.Normal)
	{
	    AlignText();
	    return;
	}
	if (labelFixType == LabelFixType.KeyMap)
	{
	    AlignTextKeymap();
	}
    }

    private void Start()
    {
	if (labelFixType == LabelFixType.Normal)
	{
	    AlignText();
	    return;
	}
	if (labelFixType == LabelFixType.KeyMap)
	{
	    AlignTextKeymap();
	}
    }

    public void AlignText()
    {
	text = GetComponent<Text>();
	if (!string.IsNullOrEmpty(text.text))
	{
	    if (text.text[text.text.Length - 1] != '\n')
	    {
		Text text = this.text;
		text.text += "\n";
	    }
	    text.lineSpacing = -0.33f;
	}
    }

    public void AlignTextKeymap()
    {
	text = GetComponent<Text>();
	if (!string.IsNullOrEmpty(text.text))
	{
	    if (text.text[text.text.Length - 1] != '\n')
	    {
		Text text = this.text;
		text.text += "\n";
	    }
	    text.lineSpacing = -0.05f;
	}
    }

    public enum LabelFixType
    {
	Normal,
	KeyMap
    }
}

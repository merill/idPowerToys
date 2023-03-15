using Syncfusion.Presentation;

namespace CADocGen.PowerPointGenerator;

public class PowerPointHelper
{
    ISlide _slide;
    Dictionary<string, IShape> _shapes = new Dictionary<string, IShape>();

    public PowerPointHelper(ISlide slide)
    {
        _slide = slide;
        InitializeShapes();
    }

    private void InitializeShapes()
    {
        foreach (IShape shape in _slide.Shapes)
        {
            _shapes.Add(shape.ShapeName, shape);
        }
    }

    public void SetText(Shape shape, string? text)
    {
        _shapes[shape.ToString()].TextBody.Text = text;
    }

    public void SetTextFormatted(Shape shape, string? text)
    {
        var textBody = _shapes[shape.ToString()].TextBody;
        for (int i = 0; i <= textBody.Paragraphs.Count(); i++) { textBody.Paragraphs.RemoveAt(0); }

        if (!string.IsNullOrEmpty(text))
        {
            var lines = text.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                var para = textBody.AddParagraph(line);
                para.Font.FontSize = 11;
                if (line.IndexOf("-") < 0)
                {
                    para.Font.Bold = true;
                }
            }
        }
    }

    public void Show(bool isShow, params Shape[] shape)
    {
        if (!isShow)
        {
            foreach (var s in shape)
            {
                Remove(s);
            }
        }
    }

    public void Remove(Shape shape)
    {
        _slide.Shapes.Remove(_shapes[shape.ToString()]);
    }

    internal void SetLink(Shape shape, string url)
    {
        _shapes[shape.ToString()].SetHyperlink(url);
    }
}

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
        foreach(IShape shape in _slide.Shapes)
        {
            _shapes.Add(shape.ShapeName, shape);
        }
    }

    public void SetText (Shape shape, string? text)
    {
        _shapes[shape.ToString()].TextBody.Text = text;
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

}

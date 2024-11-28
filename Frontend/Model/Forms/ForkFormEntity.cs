namespace ForkFrontend.Model.Forms;

public class ForkFormEntity<T, TU>
{
    public ForkFormEntity(T modelValue, TU viewValue, string? icon = null)
    {
        ModelValue = modelValue;
        ViewValue = viewValue;
        Icon = icon;
    }

    public T ModelValue { get; set; }
    public TU ViewValue { get; set; }

    public string? Icon { get; set; }
}
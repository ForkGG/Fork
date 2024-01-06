namespace ForkFrontend.Model.Forms;

public class ForkFormEntity<T, TU>
{
    public T? ModelValue { get; set; }
    public TU? ViewValue { get; set; }
    
    public string? Icon { get; set; }
    
    public ForkFormEntity(T modelValue, TU viewValue, string? icon = null)
    {
        ModelValue = modelValue;
        ViewValue = viewValue;
        Icon = icon;
    }

    public ForkFormEntity()
    {
        
    }
}
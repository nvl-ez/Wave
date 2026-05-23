namespace Wave.Ui.Views.FormComponents
{
    public interface IFormElement
    {
        public bool IsRequired { get; set; }
        public object? Model { get; }
        public bool Validate();
        public void SetInvalid();
        public void SetValid();
        public void SetModel(object? model);
    }
}
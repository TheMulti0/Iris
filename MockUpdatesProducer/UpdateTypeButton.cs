namespace MockUpdatesProducer
{
    public class UpdateTypeButton
    {
        public UpdateType UpdateType { get; }

        public string UpdateTypeName { get; }

        public UpdateTypeButton(UpdateType updateType)
        {
            UpdateType = updateType;
            UpdateTypeName = UpdateType.ToString();
        }
    }
}
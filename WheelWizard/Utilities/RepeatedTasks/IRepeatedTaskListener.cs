namespace CT_MKWII_WPF.Utilities.RepeatedTasks;

// NOTE: if you subscribe a page, NEVER forget to unsubscribe this page on unload
public interface IRepeatedTaskListener
{
    void OnUpdate(RepeatedTaskManager sender);
}

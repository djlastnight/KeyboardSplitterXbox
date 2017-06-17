namespace KeyboardSplitter.Helpers
{
    using System.Windows;
    using System.Windows.Media;

    public static class ParentFinder
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            // Get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // We've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            // Check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }
    }
}

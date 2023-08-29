namespace NomadicMVC.ViewModels.Pagination
{
    public class BaseFilterViewModel<T>
    {
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }

        private IEnumerable<T> entities;
        public IEnumerable<T> Entities { get { return entities; } set { entities = value; } }
    }
}

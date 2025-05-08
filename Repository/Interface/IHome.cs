using EVENT_ORGANIZER.Models;

namespace EVENT_ORGANIZER.Repository.Interface
{
    public interface IHome
    {
        string CreateOrUpdate(Register objs);
        List<Register> GetSelect();
        void Delete(int id);
        Register GetSelectOne(int id);

    }
}

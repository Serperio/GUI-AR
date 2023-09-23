public class Evento
{
   public string nombre;
   public string descripcion;
   public string img;

   public System.DateTime fecha;

    public void EventoData(string _nombre, string _descripcion, string _img, System.DateTime _fecha){
        nombre = _nombre;
        descripcion = _descripcion;
        img = _img;
        fecha = _fecha;
    }
}
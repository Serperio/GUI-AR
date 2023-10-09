public class Evento
{
   public string nombre;
   public string descripcion;
   public string img;

    /*
   public System.DateTime fecha_inicio;
   public System.DateTime fecha_termino;*/

    public string fecha_inicio;
    public string fecha_termino;

    public void EventoData(string _nombre, string _descripcion, string _img, string _fecha_inicio, string _fecha_termino)
    {
        nombre = _nombre;
        descripcion = _descripcion;
        img = _img;
        fecha_inicio = _fecha_inicio;
        fecha_termino = _fecha_termino;

    }
}
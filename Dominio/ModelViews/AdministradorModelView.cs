namespace MinimalApi.Dominio.ModelViews
{
    public record AdministradorModelView
    {
        public int Id { get; set; } = 0;
        public string Email { get; set; } = default!;
        public string Perfil { get; set; } = default!;        
    }
}
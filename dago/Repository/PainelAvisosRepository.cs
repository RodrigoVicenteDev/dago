using dago.Data;
using dago.Models.DTOs.Views;
using Microsoft.EntityFrameworkCore;

public class PainelAvisosRepository
{
    private readonly AppDbContext _db;

    public PainelAvisosRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CtrcsParadosGruDTO>> GetCtrcsParadosGRUAsync()
        => await _db.CtrcsParadosGru.FromSqlRaw("SELECT * FROM public.ctrcs_parados_gru").ToListAsync();

    public async Task<List<CtrcsParadosUndDTO>> GetCtrcsParadosUNDAsync()
        => await _db.CtrcsParadosUnd.FromSqlRaw("SELECT * FROM public.ctrcs_parados_und").ToListAsync();

    public async Task<List<CtrcsAtrasadaDTO>> GetCtrcsAtrasadaAsync()
        => await _db.CtrcsAtrasada.FromSqlRaw("SELECT * FROM public.ctrs_atrasada").ToListAsync();

    public async Task<List<CtrcsVaiAtrasarDTO>> GetCtrcsVaiAtrasarAsync()
        => await _db.CtrcsVaiAtrasar.FromSqlRaw("SELECT * FROM public.ctrs_vai_atrasar").ToListAsync();
}

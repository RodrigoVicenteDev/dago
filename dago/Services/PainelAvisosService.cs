public class PainelAvisosService
{
    private readonly PainelAvisosRepository _repo;

    public PainelAvisosService(PainelAvisosRepository repo)
    {
        _repo = repo;
    }

    public async Task<object> GetPainelAsync()
    {
        var gru = await _repo.GetCtrcsParadosGRUAsync();
        var und = await _repo.GetCtrcsParadosUNDAsync();
        var atrasadas = await _repo.GetCtrcsAtrasadaAsync();
        var vaiAtrasar = await _repo.GetCtrcsVaiAtrasarAsync();

        return new
        {
            CtrcsParadosGRU = gru,
            CtrcsParadosUND = und,
            CtrcsAtrasadas = atrasadas,
            CtrcsVaiAtrasar = vaiAtrasar
        };
    }
}

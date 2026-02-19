using MultiverseScout.Contracts;

namespace MultiverseScout.Api.Services;

public sealed class GuionService : IGuionService
{
    private static readonly string[] Plantillas = new[]
    {
        @"En un pliegue del multiverso, {NombreA} y {NombreB} se encuentran. Las dimensiones crujen a su alrededor.
{FuerzaFrase}
{VelocidadFrase}
El choque está por decidirse. ¡La comunidad del Multiverse Scout elegirá al vencedor!",
        @"—¿Tú también te has perdido entre realidades? —dice {NombreA}, mientras {NombreB} se prepara.
{FuerzaFrase}
{VelocidadFrase}
Solo uno saldrá victorioso. ¡Es hora de votar!",
        @"El Multiverso ha cruzado sus caminos: {NombreA} frente a {NombreB}. La tensión es palpable.
{FuerzaFrase}
{VelocidadFrase}
¿Quién tiene más poder? ¿Quién resistirá? Tú decides en Multiverse Scout.",
        @"¡POW! Un portal se abre. {NombreA} y {NombreB} se miran. La batalla está a punto de comenzar.
{FuerzaFrase}
{VelocidadFrase}
El encuentro más épico del multiverso. ¡Vota y proclama al ganador!"
    };

    public string Generar(BatallaDto batalla, PersonajeDto personajeA, PersonajeDto personajeB)
    {
        var rnd = Random.Shared;
        var plantilla = Plantillas[rnd.Next(Plantillas.Length)];

        var nombreA = personajeA.Nombre;
        var nombreB = personajeB.Nombre;
        var fuerzaA = personajeA.Atributos.Fuerza;
        var fuerzaB = personajeB.Atributos.Fuerza;
        var velA = personajeA.Atributos.Velocidad;
        var velB = personajeB.Atributos.Velocidad;

        var fuerzaFrase = GenerarFuerzaFrase(nombreA, nombreB, fuerzaA, fuerzaB);
        var velocidadFrase = GenerarVelocidadFrase(nombreA, nombreB, velA, velB);

        return plantilla
            .Replace("{NombreA}", nombreA)
            .Replace("{NombreB}", nombreB)
            .Replace("{FuerzaFrase}", fuerzaFrase)
            .Replace("{VelocidadFrase}", velocidadFrase);
    }

    private static string GenerarFuerzaFrase(string nombreA, string nombreB, int fuerzaA, int fuerzaB)
    {
        if (fuerzaA > fuerzaB)
            return $"{nombreA} muestra su poder: con una fuerza de {fuerzaA}, avanza imparable. {nombreB} (fuerza {fuerzaB}) tendrá que usar la cabeza.";
        if (fuerzaB > fuerzaA)
            return $"{nombreB} destaca en fuerza ({fuerzaB}). {nombreA} ({fuerzaA}) sabe que no puede ganar solo a puñetazos.";
        return $"En fuerza van igualados: {nombreA} y {nombreB} miden sus {fuerzaA} puntos. Cualquier cosa puede pasar.";
    }

    private static string GenerarVelocidadFrase(string nombreA, string nombreB, int velA, int velB)
    {
        if (velA > velB)
            return $"{nombreA} es más rápido (velocidad {velA}). {nombreB} ({velB}) tendrá que anticipar cada movimiento.";
        if (velB > velA)
            return $"{nombreB} es una blur (velocidad {velB}). {nombreA} ({velA}) no puede bajar la guardia ni un segundo.";
        return $"En velocidad están empatados: {nombreA} y {nombreB} a {velA}. El duelo será trepidante.";
    }
}

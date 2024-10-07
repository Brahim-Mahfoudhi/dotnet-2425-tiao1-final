using System;
using System.Collections.Generic;

public enum StreetEnum
{
    AFRIKALAAN,
    BATAVIABRUG,
    DECKERSTRAAT,
    DOKNOORD,
    DOORNZELESTRAAT,
    FINLANDSTRAAT,
    GASMETERLAAN,
    GOUDBLOEMSTRAAT,
    GROENDREEF,
    GROTEMUIDE,
    HALVEMAANSTRAAT,
    HAM,
    HAMERSTRAAT,
    INDUSTRIEWEG,
    KARELANTHEUNISSTRAAT,
    KIEKENBOSSTRAAT,
    KOOPVAARDIJLAAN,
    LANGERBRUGGEKAAI,
    MEULESTEDEBRUG,
    MEULESTEDEDIJK,
    MEULESTEDEHOF,
    MEULESTEDEKAAI,
    MEULESTEEDSESTEENWEG,
    MUIDEHOFSTRAAT,
    MUIDEKAAI,
    MUIDELAAN,
    MUIDEPOORT,
    NEERMEERSKAAI,
    NIEUWEVAART,
    OKTROOIPLEIN,
    SCHELDEKAAI,
    SCHOONSCHIPSTRAAT,
    SPANJAARDSTRAAT,
    STAPELPLEIN,
    VOORHAVENKAAI,
    ZEESCHIPSTRAAT,
    ZEESCHIPSTRAATJE,
    ZONGELAAN
}

public static class StreetEnumExtensions
{
    private static readonly Dictionary<StreetEnum, string> StreetNames = new Dictionary<StreetEnum, string>
    {
        { StreetEnum.AFRIKALAAN, "Afrikalaan" },
        { StreetEnum.BATAVIABRUG, "Bataviabrug" },
        { StreetEnum.DECKERSTRAAT, "Deckerstraat" },
        { StreetEnum.DOKNOORD, "Dok Noord" },
        { StreetEnum.DOORNZELESTRAAT, "Doornzelezestraat" },
        { StreetEnum.FINLANDSTRAAT, "Finlandstraat" },
        { StreetEnum.GASMETERLAAN, "Gasmeterlaan" },
        { StreetEnum.GOUDBLOEMSTRAAT, "Goudbloemstraat" },
        { StreetEnum.GROENDREEF, "Groendreef" },
        { StreetEnum.GROTEMUIDE, "Grote Muide" },
        { StreetEnum.HALVEMAANSTRAAT, "Halvemaanstraat" },
        { StreetEnum.HAM, "Ham" },
        { StreetEnum.HAMERSTRAAT, "Hamerstraat" },
        { StreetEnum.INDUSTRIEWEG, "Industrieweg" },
        { StreetEnum.KARELANTHEUNISSTRAAT, "Karel Antheunisstraat" },
        { StreetEnum.KIEKENBOSSTRAAT, "Kiekenbosstraat" },
        { StreetEnum.KOOPVAARDIJLAAN, "Koopvaardijlaan" },
        { StreetEnum.LANGERBRUGGEKAAI, "Langerbruggekaai" },
        { StreetEnum.MEULESTEDEBRUG, "Meulestredebrug" },
        { StreetEnum.MEULESTEDEDIJK, "Meulestededijk" },
        { StreetEnum.MEULESTEDEHOF, "Meulestedehof" },
        { StreetEnum.MEULESTEDEKAAI, "Meulestedekaai" },
        { StreetEnum.MEULESTEEDSESTEENWEG, "Meulesteedsesteenweg" },
        { StreetEnum.MUIDEHOFSTRAAT, "Muidehofstraat" },
        { StreetEnum.MUIDEKAAI, "Muidekaai" },
        { StreetEnum.MUIDELAAN, "Muidelaan" },
        { StreetEnum.MUIDEPOORT, "Muidepoort" },
        { StreetEnum.NEERMEERSKAAI, "Neermeerskaai" },
        { StreetEnum.NIEUWEVAART, "Nieuwe Vaart" },
        { StreetEnum.OKTROOIPLEIN, "Oktrooiplein" },
        { StreetEnum.SCHELDEKAAI, "Scheldekaai" },
        { StreetEnum.SCHOONSCHIPSTRAAT, "Schoonschipstraat" },
        { StreetEnum.SPANJAARDSTRAAT, "Spanjaardstraat" },
        { StreetEnum.STAPELPLEIN, "Stapelplein" },
        { StreetEnum.VOORHAVENKAAI, "Voorhavenkaai" },
        { StreetEnum.ZEESCHIPSTRAAT, "Zeeschipstraat" },
        { StreetEnum.ZEESCHIPSTRAATJE, "Zeeschipstraatje" },
        { StreetEnum.ZONGELAAN, "Zongelaan" }
    };

    public static string GetStreetName(this StreetEnum streetType)
    {
        return StreetNames.TryGetValue(streetType, out var streetName) ? streetName : string.Empty;
    }

    public static StreetEnum GetStreetEnum(this string streetName)
    {
        return StreetNames.FirstOrDefault(x => x.Value.Equals(streetName, StringComparison.OrdinalIgnoreCase)).Key;
    }
}


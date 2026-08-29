using MilesManager.Application.Abstractions;
using MilesManager.Domain.Lookups;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Api.Data;

/// <summary>
/// Seeds initial data into the database when it is empty.
/// </summary>
public static class SeedDataAirports
{
    /// <summary>
    /// Initializes seed data.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var airports = new[]
        {
            // Brazil
            new Airport { IataCode = "GRU", Name = "São Paulo–Guarulhos", City = "São Paulo", Country = "Brazil" },
            new Airport { IataCode = "CGH", Name = "São Paulo–Congonhas", City = "São Paulo", Country = "Brazil" },
            new Airport { IataCode = "VCP", Name = "Campinas–Viracopos", City = "Campinas", Country = "Brazil" },
            new Airport { IataCode = "GIG", Name = "Rio de Janeiro–Galeão", City = "Rio de Janeiro", Country = "Brazil" },
            new Airport { IataCode = "SDU", Name = "Rio de Janeiro–Santos Dumont", City = "Rio de Janeiro", Country = "Brazil" },
            new Airport { IataCode = "BSB", Name = "Brasília", City = "Brasília", Country = "Brazil" },
            new Airport { IataCode = "CNF", Name = "Belo Horizonte–Confins", City = "Belo Horizonte", Country = "Brazil" },
            new Airport { IataCode = "SSA", Name = "Salvador", City = "Salvador", Country = "Brazil" },
            new Airport { IataCode = "REC", Name = "Recife", City = "Recife", Country = "Brazil" },
            new Airport { IataCode = "FOR", Name = "Fortaleza", City = "Fortaleza", Country = "Brazil" },
            new Airport { IataCode = "POA", Name = "Porto Alegre", City = "Porto Alegre", Country = "Brazil" },
            new Airport { IataCode = "CWB", Name = "Curitiba", City = "Curitiba", Country = "Brazil" },
            new Airport { IataCode = "FLN", Name = "Florianópolis", City = "Florianópolis", Country = "Brazil" },
            new Airport { IataCode = "BEL", Name = "Belém", City = "Belém", Country = "Brazil" },
            new Airport { IataCode = "MAO", Name = "Manaus", City = "Manaus", Country = "Brazil" },

            // Portugal
            new Airport { IataCode = "LIS", Name = "Lisbon", City = "Lisbon", Country = "Portugal" },
            new Airport { IataCode = "OPO", Name = "Porto", City = "Porto", Country = "Portugal" },
            new Airport { IataCode = "FAO", Name = "Faro", City = "Faro", Country = "Portugal" },
            new Airport { IataCode = "FNC", Name = "Madeira", City = "Funchal", Country = "Portugal" },
            new Airport { IataCode = "PDL", Name = "Ponta Delgada", City = "Ponta Delgada", Country = "Portugal" },

            // Spain
            new Airport { IataCode = "MAD", Name = "Madrid", City = "Madrid", Country = "Spain" },
            new Airport { IataCode = "BCN", Name = "Barcelona", City = "Barcelona", Country = "Spain" },
            new Airport { IataCode = "AGP", Name = "Málaga", City = "Málaga", Country = "Spain" },
            new Airport { IataCode = "PMI", Name = "Palma de Mallorca", City = "Palma", Country = "Spain" },
            new Airport { IataCode = "VLC", Name = "Valencia", City = "Valencia", Country = "Spain" },
            new Airport { IataCode = "SVQ", Name = "Seville", City = "Seville", Country = "Spain" },

            // France
            new Airport { IataCode = "CDG", Name = "Paris Charles de Gaulle", City = "Paris", Country = "France" },
            new Airport { IataCode = "ORY", Name = "Paris Orly", City = "Paris", Country = "France" },
            new Airport { IataCode = "LYS", Name = "Lyon", City = "Lyon", Country = "France" },
            new Airport { IataCode = "NCE", Name = "Nice", City = "Nice", Country = "France" },
            new Airport { IataCode = "MRS", Name = "Marseille", City = "Marseille", Country = "France" },

            // Germany
            new Airport { IataCode = "FRA", Name = "Frankfurt", City = "Frankfurt", Country = "Germany" },
            new Airport { IataCode = "MUC", Name = "Munich", City = "Munich", Country = "Germany" },
            new Airport { IataCode = "BER", Name = "Berlin", City = "Berlin", Country = "Germany" },
            new Airport { IataCode = "DUS", Name = "Düsseldorf", City = "Düsseldorf", Country = "Germany" },
            new Airport { IataCode = "HAM", Name = "Hamburg", City = "Hamburg", Country = "Germany" },

            // United Kingdom
            new Airport { IataCode = "LHR", Name = "London Heathrow", City = "London", Country = "United Kingdom" },
            new Airport { IataCode = "LGW", Name = "London Gatwick", City = "London", Country = "United Kingdom" },
            new Airport { IataCode = "LCY", Name = "London City", City = "London", Country = "United Kingdom" },
            new Airport { IataCode = "STN", Name = "London Stansted", City = "London", Country = "United Kingdom" },
            new Airport { IataCode = "MAN", Name = "Manchester", City = "Manchester", Country = "United Kingdom" },
            new Airport { IataCode = "EDI", Name = "Edinburgh", City = "Edinburgh", Country = "United Kingdom" },

            // Benelux
            new Airport { IataCode = "AMS", Name = "Amsterdam Schiphol", City = "Amsterdam", Country = "Netherlands" },
            new Airport { IataCode = "BRU", Name = "Brussels", City = "Brussels", Country = "Belgium" },
            new Airport { IataCode = "LUX", Name = "Luxembourg", City = "Luxembourg", Country = "Luxembourg" },

            // Switzerland
            new Airport { IataCode = "ZRH", Name = "Zurich", City = "Zurich", Country = "Switzerland" },
            new Airport { IataCode = "GVA", Name = "Geneva", City = "Geneva", Country = "Switzerland" },

            // Italy
            new Airport { IataCode = "FCO", Name = "Rome Fiumicino", City = "Rome", Country = "Italy" },
            new Airport { IataCode = "MXP", Name = "Milan Malpensa", City = "Milan", Country = "Italy" },
            new Airport { IataCode = "LIN", Name = "Milan Linate", City = "Milan", Country = "Italy" },
            new Airport { IataCode = "VCE", Name = "Venice", City = "Venice", Country = "Italy" },
            new Airport { IataCode = "NAP", Name = "Naples", City = "Naples", Country = "Italy" },

            // Turkey
            new Airport { IataCode = "IST", Name = "Istanbul", City = "Istanbul", Country = "Turkey" },
            new Airport { IataCode = "SAW", Name = "Istanbul Sabiha Gökçen", City = "Istanbul", Country = "Turkey" },

            // Middle East
            new Airport { IataCode = "DXB", Name = "Dubai", City = "Dubai", Country = "United Arab Emirates" },
            new Airport { IataCode = "AUH", Name = "Abu Dhabi", City = "Abu Dhabi", Country = "United Arab Emirates" },
            new Airport { IataCode = "DOH", Name = "Doha", City = "Doha", Country = "Qatar" },
            new Airport { IataCode = "RUH", Name = "Riyadh", City = "Riyadh", Country = "Saudi Arabia" },
            new Airport { IataCode = "JED", Name = "Jeddah", City = "Jeddah", Country = "Saudi Arabia" },
            new Airport { IataCode = "TLV", Name = "Tel Aviv", City = "Tel Aviv", Country = "Israel" },

            // USA
            new Airport { IataCode = "JFK", Name = "John F. Kennedy International", City = "New York", Country = "United States" },
            new Airport { IataCode = "LGA", Name = "LaGuardia", City = "New York", Country = "United States" },
            new Airport { IataCode = "EWR", Name = "Newark Liberty", City = "Newark", Country = "United States" },
            new Airport { IataCode = "MIA", Name = "Miami", City = "Miami", Country = "United States" },
            new Airport { IataCode = "ATL", Name = "Atlanta", City = "Atlanta", Country = "United States" },
            new Airport { IataCode = "ORD", Name = "Chicago O'Hare", City = "Chicago", Country = "United States" },
            new Airport { IataCode = "DFW", Name = "Dallas/Fort Worth", City = "Dallas", Country = "United States" },
            new Airport { IataCode = "DEN", Name = "Denver", City = "Denver", Country = "United States" },
            new Airport { IataCode = "LAX", Name = "Los Angeles", City = "Los Angeles", Country = "United States" },
            new Airport { IataCode = "SFO", Name = "San Francisco", City = "San Francisco", Country = "United States" },
            new Airport { IataCode = "SEA", Name = "Seattle", City = "Seattle", Country = "United States" },
            new Airport { IataCode = "LAS", Name = "Las Vegas", City = "Las Vegas", Country = "United States" },
            new Airport { IataCode = "BOS", Name = "Boston", City = "Boston", Country = "United States" },
            new Airport { IataCode = "IAD", Name = "Washington Dulles", City = "Washington", Country = "United States" },

            // Canada
            new Airport { IataCode = "YYZ", Name = "Toronto Pearson", City = "Toronto", Country = "Canada" },
            new Airport { IataCode = "YVR", Name = "Vancouver", City = "Vancouver", Country = "Canada" },
            new Airport { IataCode = "YUL", Name = "Montreal", City = "Montreal", Country = "Canada" },

            // Mexico
            new Airport { IataCode = "MEX", Name = "Mexico City", City = "Mexico City", Country = "Mexico" },
            new Airport { IataCode = "CUN", Name = "Cancún", City = "Cancún", Country = "Mexico" },

            // South America
            new Airport { IataCode = "EZE", Name = "Buenos Aires Ezeiza", City = "Buenos Aires", Country = "Argentina" },
            new Airport { IataCode = "AEP", Name = "Buenos Aires Aeroparque", City = "Buenos Aires", Country = "Argentina" },
            new Airport { IataCode = "SCL", Name = "Santiago", City = "Santiago", Country = "Chile" },

            // Australia / New Zealand
            new Airport { IataCode = "SYD", Name = "Sydney", City = "Sydney", Country = "Australia" },
            new Airport { IataCode = "MEL", Name = "Melbourne", City = "Melbourne", Country = "Australia" },
            new Airport { IataCode = "BNE", Name = "Brisbane", City = "Brisbane", Country = "Australia" },
            new Airport { IataCode = "PER", Name = "Perth", City = "Perth", Country = "Australia" },
            new Airport { IataCode = "AKL", Name = "Auckland", City = "Auckland", Country = "New Zealand" },

            // Japan
            new Airport { IataCode = "HND", Name = "Tokyo Haneda", City = "Tokyo", Country = "Japan" },
            new Airport { IataCode = "NRT", Name = "Tokyo Narita", City = "Tokyo", Country = "Japan" },
            new Airport { IataCode = "KIX", Name = "Osaka Kansai", City = "Osaka", Country = "Japan" },
            new Airport { IataCode = "ITM", Name = "Osaka Itami", City = "Osaka", Country = "Japan" },

            // China
            new Airport { IataCode = "PEK", Name = "Beijing Capital", City = "Beijing", Country = "China" },
            new Airport { IataCode = "PKX", Name = "Beijing Daxing", City = "Beijing", Country = "China" },
            new Airport { IataCode = "PVG", Name = "Shanghai Pudong", City = "Shanghai", Country = "China" },
            new Airport { IataCode = "SHA", Name = "Shanghai Hongqiao", City = "Shanghai", Country = "China" },
            new Airport { IataCode = "CAN", Name = "Guangzhou", City = "Guangzhou", Country = "China" },
            new Airport { IataCode = "SZX", Name = "Shenzhen", City = "Shenzhen", Country = "China" },

            // Hong Kong / Taiwan / Korea
            new Airport { IataCode = "HKG", Name = "Hong Kong", City = "Hong Kong", Country = "Hong Kong" },
            new Airport { IataCode = "TPE", Name = "Taipei Taoyuan", City = "Taipei", Country = "Taiwan" },
            new Airport { IataCode = "ICN", Name = "Seoul Incheon", City = "Seoul", Country = "South Korea" },
            new Airport { IataCode = "GMP", Name = "Seoul Gimpo", City = "Seoul", Country = "South Korea" },

            // Southeast Asia
            new Airport { IataCode = "SIN", Name = "Singapore Changi", City = "Singapore", Country = "Singapore" },
            new Airport { IataCode = "BKK", Name = "Bangkok Suvarnabhumi", City = "Bangkok", Country = "Thailand" },
            new Airport { IataCode = "DMK", Name = "Bangkok Don Mueang", City = "Bangkok", Country = "Thailand" },
            new Airport { IataCode = "KUL", Name = "Kuala Lumpur", City = "Kuala Lumpur", Country = "Malaysia" },
            new Airport { IataCode = "CGK", Name = "Jakarta", City = "Jakarta", Country = "Indonesia" },
            new Airport { IataCode = "MNL", Name = "Manila", City = "Manila", Country = "Philippines" },
            new Airport { IataCode = "HAN", Name = "Hanoi", City = "Hanoi", Country = "Vietnam" },
            new Airport { IataCode = "SGN", Name = "Ho Chi Minh City", City = "Ho Chi Minh City", Country = "Vietnam" },

            // India
            new Airport { IataCode = "DEL", Name = "Delhi", City = "New Delhi", Country = "India" },
            new Airport { IataCode = "BOM", Name = "Mumbai", City = "Mumbai", Country = "India" },
            new Airport { IataCode = "BLR", Name = "Bengaluru", City = "Bengaluru", Country = "India" },
            new Airport { IataCode = "MAA", Name = "Chennai", City = "Chennai", Country = "India" },
            new Airport { IataCode = "HYD", Name = "Hyderabad", City = "Hyderabad", Country = "India" },

            // Africa
            new Airport { IataCode = "JNB", Name = "Johannesburg", City = "Johannesburg", Country = "South Africa" },
            new Airport { IataCode = "CPT", Name = "Cape Town", City = "Cape Town", Country = "South Africa" },
            new Airport { IataCode = "CAI", Name = "Cairo", City = "Cairo", Country = "Egypt" },
            new Airport { IataCode = "ADD", Name = "Addis Ababa", City = "Addis Ababa", Country = "Ethiopia" },
            new Airport { IataCode = "NBO", Name = "Nairobi", City = "Nairobi", Country = "Kenya" },
            new Airport { IataCode = "CMN", Name = "Casablanca", City = "Casablanca", Country = "Morocco" }
        };

        foreach (var airport in airports)
        {
            var exists = await context.Airports.AnyAsync(x => x.IataCode == airport.IataCode, cancellationToken);

            if (!exists)
            {
                await context.Airports.AddAsync(airport, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}

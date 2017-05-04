﻿using BatchDataEntry.Helpers;
using System.Collections.Generic;

namespace BatchDataEntry.Interfaces
{
    // Modello voce + campo integrati in un unica interfaccia per agevolarne l'implementazione a livello di codice in quanto fanno praticamente la stessa cosa
    public interface ICampo
    {
        // Attributi per il modello campo
        int Id { get; set; }
        string Nome { get; set; }
        int Posizione { get; set; }
        bool SalvaValori { get; set; } // -> IsAutocomplete
        string ValorePredefinito { get; set; }
        string TabellaSorgente { get; set; }
        bool IndicePrimario { get; set; }
        bool IndiceSecondario { get; set; }
        EnumTypeOfCampo TipoCampo { get; set; }
        int IdModello { get; set; }
        bool Riproponi { get; set; }
        bool IsDisabilitato { get; set; }

        // Attributi aggiuntivi incorporati per il modello voce
        string Valore { get; set; }
        ISuggestion ElementoSelezionato { get; set; }
        string ElementoSelezionatoValore { get; set; }
        List<ISuggestion> QueryProvider { get; set; }

        void QueryProviderSelector();
    }
}

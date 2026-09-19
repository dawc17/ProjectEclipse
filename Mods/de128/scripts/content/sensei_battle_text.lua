local sf2 = require("sf2")
-- Historical DE translations; pending until the complete story is enabled.
local values = {
    cro = {
        alias = "STARE RANE",
        title = "Stare rane",
        locked = "Završi 3 faze turnira i prethodne dijelove Senseijeve priče kako bi otključao ovu borbu.",
        description = "Senseijeva priča",
    },
    eng = {
        alias = "OLD WOUNDS",
        title = "Old Wounds",
        locked = "Complete three tournament stages and the previous parts{br}of Sensei’s Story to unlock this battle.",
        description = "Sensei’s Story",
    },
    fra = {
        alias = "VIEILLES BLESSURES",
        title = "Vieilles blessures",
        locked = "Terminer 3 combats de tournoi, ainsi que les passages précédents de l'histoire de Sensei pour débloquer cette bataille",
        description = "L'histoire du Sensei",
    },
    ger = {
        alias = "ALTE WUNDEN",
        title = "Alte Wunden",
        locked = "Schließe 3 Turnierkämpfe und die vorherigen Teile von Senseis Geschichte ab, um diesen Kampf freizuschalten",
        description = "Senseis Geschichte",
    },
    hin = {
        alias = "पुराने ज़ख्म",
        title = "पुराने ज़ख्म",
        locked = "इस लड़ाई को अनलॉक करने के लिए तीन टूर्नामेंट चरण और{br}सेंसेई की कहानी के पिछले भाग पूरे करें।",
        description = "सेंसेई की कहानी",
    },
    hun = {
        alias = "RÉGI SEBEK",
        title = "Régi sebek",
        locked = "A csata feloldásához teljesíts három bajnoki szakaszt és a Szenszei történetének{br}előző részeit.",
        description = "Sensei története",
    },
    ita = {
        alias = "VECCHIE FERITE",
        title = "Vecchie ferite",
        locked = "Completa 3 combattimenti in un torneo e le parti precedenti della storia di Sensei per sbloccare questa battaglia",
        description = "La storia del Sensei",
    },
    kor = {
        alias = "오래된 부상",
        title = "오래된 부상",
        locked = "이 전투를 해제하려면 3번의 토너먼트 대결과 센세이의 이야기 이전 파트를 모두 완료해야 합니다",
        description = "스승의 이야기",
    },
    por = {
        alias = "VELHOS{br}FERIMENTOS",
        title = "Velhos Ferimentos",
        locked = "Conclua 3 lutas de torneio e partes anteriores da história do Sensei para desbloquear essa batalha",
        description = "A história do Sensei",
    },
    rom = {
        alias = "RĂNI VECHI",
        title = "Răni Vechi",
        locked = "Finalizează trei etape de turneu și părţile anterioare{br}ale poveștii lui Sensei pentru a debloca această luptă.",
        description = "Povestea lui Sensei",
    },
    rus = {
        alias = "СТАРЫЕ РАНЫ",
        title = "Старые Раны",
        locked = "Пройди 3 стадии турнира и предыдущие части истории Сэнсея, чтобы открыть этот бой.",
        description = "История сэнсэя",
    },
    spa = {
        alias = "VIEJAS HERIDAS",
        title = "Viejas heridas",
        locked = "Completa 3 batallas del torneo y partes anteriores de la historia del Sensei para desbloquear esta batalla",
        description = "La historia del Sensei",
    },
    swe = {
        alias = "GAMLA SÅR",
        title = "Gamla sår",
        locked = "Slutför tre turneringsetapper och de tidigare delarna{br}av Senseis berättelse för att låsa upp denna strid.",
        description = "Senseis berättelse",
    },
    tur = {
        alias = "ESKİ YARALAR",
        title = "Eski Yaralar",
        locked = "Bu savaşı açmak için 3 turnuva dövüşü ve Sensey'in öyküsünün önceki kısımlarını tamamla",
        description = "Sensei'nin Hikayesi",
    },
}
local result = {}
for language, entries in pairs(values) do
    for key, value in pairs(entries) do
        sf2.localization.register { id = "sensei.battle." .. key, language = language, value = value }
        result[key] = sf2.mod.id .. ":localization/sensei.battle." .. key
    end
end
return result

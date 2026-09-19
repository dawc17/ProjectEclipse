local sf2 = require("sf2")
-- Historical DE localization values, authored here for Lua-only runtime loading.
local values = {
    cro = {
        title = "SENSEI",
        intro = "Ovaj grad budi uspomene. Ispričat ću ti priču koja mi se dogodila mnogo godina temu...",
        more = "Sjetio sam se još nečeg, vraćamo li se mojoj priči?",
        ending = "Sjetio sam se kako je sve završilo! Vraćamo li se mojoj priči?",
    },
    eng = {
        title = "SENSEI",
        intro = "This place awakens some memories. I will tell you my story—one that happened many years ago...",
        more = "I remembered something more. Shall we return to my story?",
        ending = "I remembered what happened at the end. Shall we return to my story?",
    },
    fra = {
        title = "SENSEI",
        intro = "Cette ville a éveillé certains souvenirs... Je vais te raconter une histoire qui m'est arrivée il y a fort longtemps...",
        more = "Un autre souvenir me vient, veux-tu que je continue mon histoire ?",
        ending = "Je me rappelle la manière dont les choses se sont terminées, veux-tu que je continue mon histoire ?",
    },
    ger = {
        title = "SENSEI",
        intro = "Die Stadt weckt einige Erinnerungen. Ich erzähle dir meine Geschichte, die sich vor vielen Jahren ereignete ...",
        more = "Ich erinnere mich noch an etwas – wollen wir zu meiner Geschichte zurückkehren?",
        ending = "Ich erinnere mich, was am Ende geschah – wollen wir zu meiner Geschichte zurückkehren?",
    },
    hin = {
        title = "सेंसी",
        intro = "यह जगह कुछ यादें ताज़ा करती है। मैं तुम्हें अपनी कहानी बताऊंगा—जो कई साल पहले घटी थी...",
        more = "मुझे कुछ और याद आया। क्या हम अपनी कहानी पर वापस चलें?",
        ending = "मुझे याद आया कि अंत में क्या हुआ था। क्या हम अपनी कहानी पर वापस चलें?",
    },
    hun = {
        title = "SENSEI",
        intro = "Ez a hely felébreszt néhány emléket. Elmesélem a történetemet – egyet, ami sok-sok évvel ezelőtt történt...",
        more = "Eszembe jutott még valami. Visszatérjünk a történetemhez?",
        ending = "Eszembe jutott, mi történt a végén. Visszatérjünk a történetemhez?",
    },
    ita = {
        title = "SENSEI",
        intro = "Questa città ha risvegliato alcuni dei miei ricordi. Ti racconterò la mia storia. Questi eventi sono accaduti molto tempo fa...",
        more = "Ho ricordato un'altra parte della mia storia. Vuoi che continui il mio racconto?",
        ending = "Mi sono ricordato cos'è successo alla fine. Vuoi che continui il mio racconto?",
    },
    kor = {
        title = "센세이",
        intro = "이 도시를 보니 옛 기억이 떠오르는군. 그대에게 오래전에 있었던 이야기를 들려주지...",
        more = "아직 할 얘기가 더 있는데, 다시 이야기로 돌아가겠나?",
        ending = "아직 마지막 이야기를 하지 못했는데, 다시 이야기로 돌아가겠나?",
    },
    por = {
        title = "SENSEI",
        intro = "Essa cidade despertou algumas de minhas lembranças. Vou contar minha história para você, que aconteceu muitos anos atrás...",
        more = "Eu me lembrei de mais um pouco. Devo retomar a história?",
        ending = "Eu me lembrei do que aconteceu no fim! Devo retomar minha história?",
    },
    rom = {
        title = "SENSEI",
        intro = "Acest loc trezește niște amintiri. Îţi voi spune povestea mea — una care s-a întâmplat cu mulţi ani în urmă...",
        more = "Mi-am amintit ceva mai mult. Să ne întoarcem la povestea mea?",
        ending = "Mi-am amintit ce s-a întâmplat la final. Să ne întoarcem la povestea mea?",
    },
    rus = {
        title = "СЭНСЕЙ",
        intro = "Этот город будит воспоминания. Я расскажу тебе историю, которая случилась со мной много лет назад...",
        more = "Я вспомнил еще кое-что, вернемся к моей истории?",
        ending = "Я вспомнил, чем все закончилось! Вернемся к моей истории?",
    },
    spa = {
        title = "SENSEI",
        intro = "La ciudad despierta algunos recuerdos. Te contaré mi historia, que ocurrió hace muchos años...",
        more = "Me he acordado de algo más, ¿volvemos a mi relato?",
        ending = "Me he acordado de lo que ocurrió al final, ¿volvemos a mi relato?",
    },
    swe = {
        title = "SENSEI",
        intro = "Platsen väcker en del minnen till liv. Jag ska berätta min historia för dig — en som utspelade sig för många år sedan...",
        more = "Jag kom ihåg något mer. Ska vi återgå till min berättelse?",
        ending = "Jag kom ihåg vad som hände i slutet. Ska vi återgå till min berättelse?",
    },
    tur = {
        title = "SENSEY",
        intro = "Bu şehirde bazı hatıralar uyanıyor. Yıllar önce olan öykümü sana anlatayım...",
        more = "Bir şey daha hatırladım, öyküme dönelim mi?",
        ending = "Sonunda ne olduğunu hatırladım, öyküme dönelim mi?",
    },
}
local result = {}
for language, entries in pairs(values) do
    for key, value in pairs(entries) do
        result[key] = sf2.localization.register { id = "sensei.notify." .. key, language = language, value = value }
    end
end
return result

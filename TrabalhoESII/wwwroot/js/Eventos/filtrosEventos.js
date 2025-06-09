window.addEventListener("DOMContentLoaded", () => {
    // Botão de pesquisa
    const btnPesquisar = document.getElementById("btnPesquisar");
    console.log("🔍 Botão:", btnPesquisar);

    if (!btnPesquisar) {
        console.warn("❌ Botão de pesquisa não encontrado!");
        return;
    }

    btnPesquisar.addEventListener("click", async () => {
        console.log("🔎 Clicado no botão de pesquisa");

        const nome = document.getElementById("searchNome")?.value;
        const data = document.getElementById("searchData")?.value;
        const local = document.getElementById("searchLocal")?.value;
        const idCategoria = document.getElementById("searchCategoria")?.value;

        const params = new URLSearchParams();
        if (nome) params.append("nome", nome);
        if (data) params.append("data", data);
        if (local) params.append("local", local);
        if (idCategoria) params.append("idCategoria", idCategoria);

        try {
            const response = await fetch(`/api/eventos/search?${params.toString()}`, {
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("jwtToken")}`
                }
            });

            if (!response.ok) throw new Error("Erro ao buscar eventos.");

            const eventos = await response.json();
            const auth = await getUserIdAndType(); // Função já existente no projeto
            renderizarEventos(eventos, auth.userId, auth.userType); // Função já existente
        } catch (err) {
            console.error("❌ Erro ao filtrar eventos:", err);
            Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: 'Erro ao pesquisar eventos.',
            });
        }
    });

    // Filtro inicial
    filterEvents('all');

    // Eventos para os botões "Todos", "Futuros", "Passados"
    const tabs = document.querySelectorAll('.event-tabs .nav-link');
    tabs.forEach(tab => {
        tab.addEventListener("click", (e) => {
            e.preventDefault();
            const tipoRaw = tab.textContent.trim().toLowerCase();
            // mapeia texto → tipo interno
            let tipo;
            switch (tipoRaw) {
                case 'todos':     tipo = 'all';       break;
                case 'futuros':   tipo = 'upcoming';  break;
                case 'passados':  tipo = 'past';      break;
                case 'inscritos': tipo = 'subscribed'; break;
                case 'organizador': tipo = 'organizer'; break;
                default:          tipo = tipoRaw;     break;
            }
            filterEvents(tipo);
        });
    });
});

// Função global para filtrar por data
window.filterEvents = function (tipo) {
    const hoje = new Date();

    // Ativar aba correta
    document.querySelectorAll('.event-tabs .nav-link').forEach(link => {
        link.classList.remove('active');
    });
    const ativo = document.querySelector(`.event-tabs .nav-link[onclick="filterEvents('${tipo}')"]`);
    if (ativo) ativo.classList.add('active');

    // Filtragem de cada cartão
    document.querySelectorAll('.event-card').forEach(card => {
        // 1) sempre pegamos a data para os filtros de data
        const textInfo = card.querySelector('.text-muted')?.textContent;
        if (!textInfo) {
            card.style.display = 'none';
            return;
        }
        const [dia, mes, ano] = textInfo.split("·")[0].trim().split('/');
        const dataEvento = new Date(`${ano}-${mes}-${dia}T00:00:00`);

        if (tipo === 'all') {
            card.style.display = 'block';
        }
        else if (tipo === 'upcoming') {
            card.style.display = dataEvento >= hoje ? 'block' : 'none';
        }
        else if (tipo === 'past') {
            card.style.display = dataEvento < hoje ? 'block' : 'none';
        }
        else if (tipo === 'subscribed') {
            // pressupõe que renderizarEventos tenha adicionado data-inscrito="true|false"
            card.style.display = card.getAttribute('data-inscrito') === 'true'
                ? 'block'
                : 'none';
        }
        else if (tipo === 'organizer') {
            // pressupõe que renderizarEventos tenha adicionado data-eorganizador="true|false"
            card.style.display = card.getAttribute('data-eorganizador') === 'true'
                ? 'block'
                : 'none';
        }
        else {
            card.style.display = 'none';
        }
    });
};

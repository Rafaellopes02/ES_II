window.addEventListener("DOMContentLoaded", () => {
    const btnPesquisar = document.getElementById("btnPesquisar");
    const btnLimpar    = document.getElementById("btnLimpar");
    const startDate    = document.getElementById("startDate");
    const endDate      = document.getElementById("endDate");
    const tabs         = document.querySelectorAll(".event-tabs .nav-link");

    if (!btnPesquisar || !btnLimpar) {
        return;
    }

    startDate.addEventListener("change", () => {
        if (startDate.value) {
            endDate.disabled = false;
            endDate.min = startDate.value;
            if (endDate.value && endDate.value < startDate.value) {
                endDate.value = "";
            }
        } else {
            endDate.disabled = true;
            endDate.value = "";
        }
    });

    btnLimpar.addEventListener("click", (e) => {
        e.preventDefault();
        document.getElementById("searchNome").value = "";
        document.getElementById("searchLocal").value = "";
        document.getElementById("searchCategoria").value = "";
        startDate.value = "";
        endDate.value   = "";
        endDate.disabled = true;
        endDate.min      = "";
        tabs.forEach(t => t.classList.remove("active"));
        const abaTodos = document.querySelector(".event-tabs .nav-link[onclick=\"filterEvents('all')\"]");
        if (abaTodos) abaTodos.classList.add("active");
        btnPesquisar.click();
    });

    btnPesquisar.addEventListener("click", async (e) => {
        e.preventDefault();
        const nome        = document.getElementById("searchNome")?.value;
        const local       = document.getElementById("searchLocal")?.value;
        const idCategoria = document.getElementById("searchCategoria")?.value;
        const dataInicio  = startDate.value;
        const dataFim     = endDate.value;

        const params = new URLSearchParams();
        if (nome)        params.append("nome", nome);
        if (local)       params.append("local", local);
        if (idCategoria) params.append("idCategoria", idCategoria);

        try {
            const response = await fetch(
                `/api/eventos/search?${params.toString()}`,
                {
                    credentials: "include"
                }
            );
            
            if (!response.ok) throw new Error("Erro ao pesquisar eventos");

            const result = await response.json();
            const auth = await getUserIdAndType();
            renderizarEventos(result, auth.userId, auth.userType);

            const activeTab = document.querySelector(".event-tabs .nav-link.active");
            const tipoRaw = activeTab?.textContent.trim().toLowerCase();
            let tipo;
            switch (tipoRaw) {
                case 'todos':      tipo = 'all';        break;
                case 'futuros':    tipo = 'upcoming';   break;
                case 'passados':   tipo = 'past';       break;
                case 'inscritos':  tipo = 'subscribed';  break;
                case 'organizador':tipo = 'organizer';break;
                default:           tipo = 'all';
            }
            filterEvents(tipo);

            if (dataInicio && dataFim) {
                const ini = new Date(dataInicio).getTime();
                const fim = new Date(dataFim).getTime();
                document.querySelectorAll(".event-card").forEach(card => {
                    const dt = new Date(card.dataset.date).getTime();
                    if (dt < ini || dt > fim) {
                        card.style.display = "none";
                    }
                });
            }
        } catch (err) {
            console.error("Erro ao filtrar eventos:", err);
            Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: 'Não foi possível realizar a pesquisa.'
            });
        }
    });

    btnPesquisar.click();
});

window.filterEvents = function (tipo) {
    const hoje = new Date().setHours(0, 0, 0, 0);
    document.querySelectorAll('.event-tabs .nav-link').forEach(link => link.classList.remove('active'));
    const ativo = document.querySelector(`.event-tabs .nav-link[onclick="filterEvents('${tipo}')"]`);
    if (ativo) ativo.classList.add('active');

    document.querySelectorAll('.event-card').forEach(card => {
        const textInfo = card.querySelector('.text-muted')?.textContent;
        if (!textInfo) { card.style.display = 'none'; return; }
        const [dia, mes, ano] = textInfo.split('·')[0].trim().split('/');
        const dataEvento = new Date(`${ano}-${mes}-${dia}T00:00:00`);

        let show = false;
        switch (tipo) {
            case 'all':     show = true; break;
            case 'upcoming':show = dataEvento >= hoje; break;
            case 'past':    show = dataEvento < hoje; break;
            case 'subscribed':    show = card.dataset.inscrito === 'true'; break;
            case 'organizer':  show = card.dataset.eorganizador === 'true'; break;
        }
        card.style.display = show ? 'block' : 'none';
    });
};

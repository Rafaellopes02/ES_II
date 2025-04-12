window.addEventListener("DOMContentLoaded", () => {
    filterEvents('all');
    const tabs = document.querySelectorAll('[data-filter]');
    tabs.forEach(tab => {
        tab.addEventListener("click", function (e) {
            e.preventDefault();
            const tipo = this.getAttribute("data-filter");
            filterEvents(tipo);
        });
    });
});

window.filterEvents = function (tipo) {
    const hoje = new Date();

    // Atualiza o estilo dos botões
    document.querySelectorAll('.event-tabs .nav-link').forEach(link => {
        link.classList.remove('active');
    });

    const ativo = document.querySelector(`.event-tabs .nav-link[onclick="filterEvents('${tipo}')"]`);
    if (ativo) {
        ativo.classList.add('active');
    }

    // Lógica de filtragem
    const cards = document.querySelectorAll('.event-card');
    cards.forEach(card => {
        const textInfo = card.querySelector('.text-muted')?.textContent;
        if (!textInfo) {
            card.style.display = 'none';
            return;
        }

        const dataTexto = textInfo.split("·")[0]?.trim();
        const [dia, mes, ano] = dataTexto.split('/');
        const dataEvento = new Date(`${ano}-${mes}-${dia}T00:00:00`);

        if (tipo === 'all') {
            card.style.display = 'block';
        } else if (tipo === 'upcoming') {
            card.style.display = dataEvento >= hoje ? 'block' : 'none';
        } else if (tipo === 'past') {
            card.style.display = dataEvento < hoje ? 'block' : 'none';
        }
    });
};


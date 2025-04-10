document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("jwtToken");

    if (!token) {
        window.location.href = "/login";
        return;
    }

    await carregarEventos(token);
    await carregarCategorias();

    // Evento do botão de pesquisa
    document.getElementById("searchBtn").addEventListener("click", async () => {
        const nome = document.getElementById("searchNome").value;
        const data = document.getElementById("searchData").value;
        const local = document.getElementById("searchLocal").value;
        const idCategoria = document.getElementById("searchCategoria").value;

        const params = new URLSearchParams();
        if (nome) params.append("nome", nome);
        if (data) params.append("data", data);
        if (local) params.append("local", local);
        if (idCategoria) params.append("idCategoria", idCategoria);

        try {
            const response = await fetch(`/api/eventos/search?${params.toString()}`, {
                headers: { "Authorization": `Bearer ${token}` }
            });

            if (response.ok) {
                const eventos = await response.json();
                renderizarEventos(eventos);
            } else {
                alert("Erro ao filtrar eventos.");
            }
        } catch (err) {
            console.error("Erro na pesquisa:", err);
            alert("Erro de comunicação com o servidor.");
        }
    });
});

// Carrega todos os eventos inicialmente
async function carregarEventos(token) {
    try {
        const response = await fetch("/eventos/stats", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.ok) {
            const data = await response.json();
            renderizarEventos(data.eventos);
        } else {
            throw new Error("Erro ao buscar eventos.");
        }
    } catch (error) {
        console.error("Erro ao carregar eventos:", error);
    }
}

// Carrega categorias no dropdown de filtro
async function carregarCategorias() {
    try {
        const response = await fetch("/api/categorias");
        if (!response.ok) throw new Error("Erro ao buscar categorias.");

        const categorias = await response.json();
        const select = document.getElementById("searchCategoria");

        categorias.forEach(c => {
            const option = document.createElement("option");
            option.value = c.idcategoria;
            option.textContent = c.nome;
            select.appendChild(option);
        });
    } catch (error) {
        console.error("Erro ao carregar categorias:", error);
    }
}

// Função para mostrar os eventos na página
function renderizarEventos(eventos) {
    const eventList = document.getElementById("eventList");
    eventList.innerHTML = "";

    if (!eventos || eventos.length === 0) {
        eventList.innerHTML = `<p class="text-muted">Nenhum evento encontrado.</p>`;
        return;
    }

    eventos.forEach(evento => {
        const formattedDate = new Date(evento.data).toLocaleDateString('pt-PT');

        const card = document.createElement("div");
        card.className = "card mb-3 p-3 event-card";

        card.innerHTML = `
            <h5 class="card-title">${evento.nome}</h5>
            <p class="card-text">${evento.descricao}</p>
            <p class="card-text">
                <small class="text-muted">${formattedDate} · ${evento.hora} · ${evento.local}</small>
            </p>
            <div class="progress">
                <div class="progress-bar bg-info" style="width: 75%">
                    75% da capacidade
                </div>
            </div>
            <div class="mt-3">
                <button class="btn btn-outline-primary" onclick="location.href='/events/details/${evento.idevento}'">
                    Detalhes
                </button>
            </div>
        `;

        eventList.appendChild(card);
    });
}

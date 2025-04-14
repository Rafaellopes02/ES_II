document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("jwtToken");

    if (!token) {
        window.location.href = "/login";
        return;
    }

    try {
        const response = await fetch("/eventos/stats", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.ok) {
            const data = await response.json();
            renderizarEventos(data.eventos);
        } else if (response.status === 401) {
            await Swal.fire({
                icon: 'warning',
                title: 'Sessão Expirada',
                text: 'Por favor, inicie sessão novamente.',
                timer: 2000,
                showConfirmButton: false
            });
            localStorage.removeItem("jwtToken");
            window.location.href = "/login";
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: 'Erro ao buscar eventos.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    } catch (error) {
        console.error("Erro ao carregar eventos:", error);
        await Swal.fire({
            icon: 'error',
            title: 'Erro de ligação',
            text: 'Erro de comunicação com o servidor.',
            timer: 2000,
            showConfirmButton: false
        });
    }

    const botaoPesquisa = document.getElementById("searchBtn");
    if (botaoPesquisa) {
        botaoPesquisa.addEventListener("click", async () => {
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
                    await Swal.fire({
                        icon: 'error',
                        title: 'Erro ao pesquisar',
                        text: 'Não foi possível encontrar os eventos com os filtros aplicados.',
                        timer: 2000,
                        showConfirmButton: false
                    });
                }
            } catch (err) {
                console.error("Erro:", err);
                await Swal.fire({
                    icon: 'error',
                    title: 'Erro de ligação',
                    text: 'Erro de ligação ao servidor.',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        });
    }

    carregarCategorias("searchCategoria");
    carregarCategorias("eventCategory");
    carregarCategorias("editEventCategory");
});

function getUserIdFromToken() {
    const token = localStorage.getItem("jwtToken");
    if (!token) return null;

    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return parseInt(payload.UserId);
    } catch (err) {
        console.error("Erro ao extrair UserId do token:", err);
        return null;
    }
}

function getUserTypeFromToken() {
    const token = localStorage.getItem("jwtToken");
    if (!token) return null;

    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return parseInt(payload.TipoUtilizadorId); // deve bater com o nome no backend
    } catch (err) {
        console.error("Erro ao extrair TipoUtilizadorId do token:", err);
        return null;
    }
}

function renderizarEventos(eventos) {
    const userId = getUserIdFromToken();
    const userType = getUserTypeFromToken(); // ← novo
    const eventList = document.getElementById("eventList");
    eventList.innerHTML = "";

    eventos.forEach(evento => {
        const formattedDate = new Date(evento.data).toLocaleDateString('pt-PT');
        const card = document.createElement("div");
        card.className = "card mb-3 p-3 event-card";

        // Botões comuns
        let botoes = `
            <button class="btn btn-outline-primary" onclick="carregarDetalhesEvento(${evento.idevento})">
                Detalhes
            </button>
            <div id="detalhes-${evento.idevento}" style="display: none;"></div>
        `;

        // Mostrar botões apenas se for o criador ou admin
        if (evento.idutilizador === userId || userType === 1) {
            botoes += `
                <button 
                    class="btn btn-outline-primary editar-btn"
                    data-id="${evento.idevento}"
                    data-nome="${evento.nome}"
                    data-descricao="${evento.descricao}"
                    data-data="${evento.data}"
                    data-hora="${evento.hora}"
                    data-local="${evento.local}"
                    data-capacidade="${evento.capacidade}"
                    data-categoria="${evento.idcategoria}">
                    Editar
                </button>
                <button 
                    class="btn btn-outline-danger eliminar-btn"
                    data-id="${evento.idevento}">
                    Eliminar
                </button>
            `;
        }

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
            <div class="mt-3">${botoes}</div>
        `;

        eventList.appendChild(card);
    });
}

function carregarDetalhesEvento(id) {
    const detalhesDiv = document.getElementById(`detalhes-${id}`);

    if (detalhesDiv.style.display === "block") {
        detalhesDiv.style.display = "none";
        return;
    }

    fetch(`/api/eventos/detalhes/${id}`, {
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("jwtToken")}`
        }
    })
        .then(res => {
            if (!res.ok) {
                throw new Error("Erro ao buscar detalhes do evento.");
            }
            return res.json();
        })
        .then(data => {
            const dataOriginal = data.data.split("T")[0];
            const horaOriginal = data.hora;

            detalhesDiv.innerHTML = `
                <div class="card card-body mt-2">
                    <p><strong>Nome:</strong> ${data.nome}</p>
                    <p><strong>Descrição:</strong> ${data.descricao}</p>
                    <p><strong>Data:</strong> ${dataOriginal} ${horaOriginal}</p>
                    <p><strong>Local:</strong> ${data.local}</p>
                    <p><strong>Categoria:</strong> ${data.categoriaNome}</p>
                    <p><strong>Capacidade:</strong> ${data.capacidade}</p>
                    <p><strong>Inscritos:</strong> ${data.inscritos ?? "N/A"}</p>
                </div>
            `;
            detalhesDiv.style.display = "block";
        })
        .catch(async error => {
            console.error("Erro ao carregar detalhes:", error);
            await Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: 'Erro ao carregar os detalhes do evento.',
                timer: 2000,
                showConfirmButton: false
            });
        });
}

async function carregarCategorias(selectId) {
    try {
        const response = await fetch("/api/categorias");
        if (response.ok) {
            const categorias = await response.json();
            const select = document.getElementById(selectId);
            if (!select) return;

            select.innerHTML = '<option value="">Selecionar...</option>';

            categorias.forEach(categoria => {
                const option = document.createElement("option");
                option.value = categoria.idcategoria;
                option.textContent = categoria.nome;
                select.appendChild(option);
            });
        } else {
            console.warn("Erro ao carregar categorias");
        }
    } catch (err) {
        console.error("Erro ao carregar categorias:", err);
    }
}

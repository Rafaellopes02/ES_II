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
    const userType = getUserTypeFromToken();
    const eventList = document.getElementById("eventList");
    eventList.innerHTML = "";

    eventos.forEach(evento => {
        const formattedDate = new Date(evento.data).toLocaleDateString('pt-PT');
        const eventoPassado = new Date(evento.data) < new Date().setHours(0, 0, 0, 0);
        const inscritos = evento.inscritos ?? 0;
        const percentagem = Math.min(100, Math.round((inscritos / evento.capacidade) * 100));

        const corBarra = percentagem === 100 ? "bg-primary" : "bg-info";
        const barra = `
            <div class="progress">
                <div class="progress-bar ${corBarra}" style="width: ${percentagem}%">
                    ${percentagem}% da capacidade
                </div>
            </div>
        `;

        let botoes = `
            <button class="btn btn-outline-primary" onclick="carregarDetalhesEvento(${evento.idevento})">
                Detalhes
            </button>
            <div id="detalhes-${evento.idevento}" style="display: none;"></div>
        `;

        // Botões para criador (eorganizador) ou admin
        if ((evento.eorganizador && evento.idutilizador === userId) || userType === 1) {
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

        // Botão "Entrar" se estiver inscrito, for criador ou admin
        if (
            (evento.inscrito && userType === 3) ||
            (evento.eorganizador && evento.idutilizador === userId) ||
            userType === 1
        ) {
            botoes += `
                <button 
                    class="btn btn-outline-success Entrar-btn"
                    onclick="window.location.href='/Eventos/${evento.idevento}'">
                    Entrar
                </button>
            `;
        }

        // Botão "Inscrever-me" se não estiver inscrito, for participante, tiver vagas e evento não for passado
        if (!evento.inscrito && userType === 3 && inscritos < evento.capacidade && !eventoPassado) {
            botoes += `
                <button 
                    class="btn btn-outline-success inscrever-btn"
                    data-id="${evento.idevento}">
                    Inscrever-me
                </button>
            `;
        }

        let cancelarBotaoHTML = '';
        if (
            userType === 3 &&
            evento.inscrito === true &&
            evento.eorganizador !== true &&
            !eventoPassado
        ) {
            cancelarBotaoHTML = `
        <button 
            class="btn btn-outline-warning cancelar-btn"
            data-id="${evento.idevento}">
            Cancelar Inscrição
        </button>
    `;
        }



        const card = document.createElement("div");
        card.className = "card mb-3 p-3 event-card";
        card.innerHTML = `
            <h5 class="card-title">${evento.nome}</h5>
            <p class="card-text">${evento.descricao}</p>
            <p class="card-text">
                <small class="text-muted">${formattedDate} · ${evento.hora} · ${evento.local}</small>
            </p>
            ${barra}
            <div class="mt-3 d-flex justify-content-between flex-wrap gap-2">
                <div class="d-flex flex-wrap gap-2">
                    ${botoes.replace(cancelarBotaoHTML, '')}
                </div>
                <div>
                ${cancelarBotaoHTML}
                </div>
             </div>

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

document.body.addEventListener("click", async (e) => {
    if (e.target.classList.contains("inscrever-btn")) {
        const id = e.target.dataset.id;

        const confirmacao = await Swal.fire({
            title: "Confirmar inscrição?",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Sim, inscrever-me",
            cancelButtonText: "Cancelar"
        });

        if (!confirmacao.isConfirmed) return;

        const token = localStorage.getItem("jwtToken");
        if (!token) {
            await Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'warning',
                title: 'Sessão expirada',
                text: 'Por favor, inicie sessão.',
                timer: 2000,
                showConfirmButton: false
            });
            window.location.href = "/login";
            return;
        }

        try {
            const response = await fetch(`/api/eventos/${id}/inscrever`, {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            if (response.ok) {
                await Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Inscrito com sucesso!',
                    timer: 2000,
                    showConfirmButton: false
                });

                location.reload();
            } else {
                const msg = await response.text();
                await Swal.fire({
                    icon: 'error',
                    title: 'Erro ao inscrever',
                    text: msg,
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        } catch (err) {
            console.error("Erro ao inscrever:", err);
            await Swal.fire({
                icon: 'error',
                title: 'Erro de ligação',
                text: 'Erro ao comunicar com o servidor.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    }

    // ✅ Cancelar inscrição — FORA do outro listener
    if (e.target.classList.contains("cancelar-btn")) {
        const id = e.target.dataset.id;

        const confirmacao = await Swal.fire({
            title: "Cancelar inscrição?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sim, cancelar",
            cancelButtonText: "Não"
        });

        if (!confirmacao.isConfirmed) return;

        try {
            const response = await fetch(`/api/eventos/${id}/cancelar`, {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("jwtToken")}`
                }
            });

            if (response.ok) {
                await Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Inscrição cancelada!',
                    timer: 2000,
                    showConfirmButton: false
                });
                location.reload();
            } else {
                const msg = await response.text();
                await Swal.fire({
                    icon: 'error',
                    title: 'Erro ao cancelar',
                    text: msg,
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        } catch (err) {
            console.error("Erro ao cancelar inscrição:", err);
            await Swal.fire({
                icon: 'error',
                title: 'Erro de ligação',
                text: 'Erro ao comunicar com o servidor.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    }
});

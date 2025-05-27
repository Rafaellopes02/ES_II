document.addEventListener("DOMContentLoaded", async () => {
    await carregarAtividades();
});

function getCookieToken() {
    const name = "jwtToken=";
    const decoded = decodeURIComponent(document.cookie);
    const parts = decoded.split(';');

    for (let i = 0; i < parts.length; i++) {
        let c = parts[i].trim();
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }

    return null;
}

function getUserIdFromToken() {
    const token = getCookieToken();
    if (!token) return null;

    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return parseInt(payload.UserId);
    } catch (err) {
        console.error("Erro ao extrair UserId:", err);
        return null;
    }
}

function getUserTypeFromToken() {
    const token = getCookieToken();
    if (!token) return null;

    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return parseInt(payload.TipoUtilizadorId);
    } catch (err) {
        console.error("Erro ao extrair TipoUtilizadorId:", err);
        return null;
    }
}

async function carregarAtividades() {
    const token = getCookieToken();
    const idevento = parseInt(document.getElementById("ideventoHidden").value);

    try {
        const response = await fetch(`/api/atividades/search-atividades?idevento=${idevento}`, {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) throw new Error("Erro ao buscar atividades.");

        const atividades = await response.json();
        const listaDiv = document.getElementById("listaAtividades");
        listaDiv.innerHTML = "";

        if (atividades.length === 0) {
            listaDiv.innerHTML = `<p class="text-muted">Nenhuma atividade registada para este evento.</p>`;
            return;
        }

        atividades.forEach(atividade => {
            const card = document.createElement("div");
            card.className = "card mb-3 p-3";

            let botaoHtml = "";
            if (atividade.inscrito) {
                botaoHtml = `<button class="btn btn-outline-danger cancelar-inscricao-btn" data-id="${atividade.idatividade}">Cancelar inscrição</button>`;
            } else {
                botaoHtml = `<button class="btn btn-outline-success inscrever-btn" data-id="${atividade.idatividade}">Inscrever-me</button>`;
            }

            card.innerHTML = `
        <div class="d-flex justify-content-between align-items-center">
            <div>
                <h5 class="card-title mb-1">${atividade.nome}</h5>
                <p class="mb-1"><strong>Data:</strong> ${atividade.data} <strong>Hora:</strong> ${atividade.hora}</p>
                <p class="mb-1"><strong>Quantidade Máxima:</strong> ${atividade.quantidademaxima}</p>
            </div>
            ${botaoHtml}
        </div>
    `;

            listaDiv.appendChild(card);
        });
    } catch (error) {
        console.error(error);
        Swal.fire("Erro", "Não foi possível carregar as atividades.", "error");
    }
}

async function submeterAtividade() {
    const token = getCookieToken();
    const idevento = parseInt(document.getElementById("ideventoHidden").value);
    const capacidadeEvento = parseInt(document.getElementById("capacidadeEventoHidden").value);

    const nome = document.getElementById("nomeAtividade").value;
    const quantidademaxima = parseInt(document.getElementById("quantidadeMaxima").value);
    const data = document.getElementById("dataAtividade").value;
    const hora = document.getElementById("horaAtividade").value;

    // No EventoDetalhes.js, antes de submeter
    const dataString = document.getElementById("dataAtividade").value; // '2025-05-27'
    const dataEventoHidden = document.getElementById("dataEventoHidden").value; // '27/05/2025'

    // Converter dataEventoHidden para ISO (yyyy-MM-dd)
    function converterParaISO(dataBR) {
        const partes = dataBR.split('/');
        if (partes.length === 3) {
            // [dia, mês, ano]
            return `${partes[2]}-${partes[1].padStart(2, '0')}-${partes[0].padStart(2, '0')}`;
        }
        return dataBR; // fallback se não estiver no formato esperado
    }

    const dataEvento = converterParaISO(dataEventoHidden);
    // Agora use dataEvento nas validações de JS e, SE PRECISAR enviar ao servidor, envie sempre no formato ISO.

    // Validação frontend
    if (data < dataEvento) {
        await Swal.fire("Erro", "A data da atividade não pode ser anterior à data do evento.", "error");
        return;
    }
    if (quantidademaxima > capacidadeEvento) {
        await Swal.fire("Erro", "A capacidade da atividade não pode ser superior à capacidade do evento.", "error");
        return;
    }

    const atividade = { nome, quantidademaxima, data, hora, idevento };

    try {
        const response = await fetch("/api/atividades/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(atividade)
        });

        if (response.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Atividade adicionada!',
                showConfirmButton: false,
                timer: 2000
            });

            document.getElementById("formNovaAtividade").reset();
            const modalEl = document.getElementById('modalNovaAtividade');
            const modal = bootstrap.Modal.getInstance(modalEl);
            modal.hide();

            const backdrops = document.querySelectorAll('.modal-backdrop');
            backdrops.forEach(bd => bd.remove());

            document.body.classList.remove('modal-open');
            document.body.style = "";

            carregarAtividades();
        } else {
            const msg = await response.text();
            await Swal.fire("Erro", msg, "error");
        }
    } catch (err) {
        console.error("Erro ao registar atividade:", err);
        await Swal.fire("Erro", "Falha ao comunicar com o servidor", "error");
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

        const token = getCookieToken();

        try {
            const response = await fetch(`/api/atividades/${id}/inscrever`, {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            if (response.ok) {
                await Swal.fire({
                    icon: "success",
                    title: "Inscrição realizada!",
                    timer: 2000,
                    showConfirmButton: false
                });

                e.target.style.display = "none";
                await carregarAtividades(); // Atualiza a lista
            } else {
                const msg = await response.text();
                Swal.fire("Erro", msg, "error");
            }
        } catch (err) {
            console.error(err);
            Swal.fire("Erro", "Erro ao comunicar com o servidor", "error");
        }
    }

    if (e.target.classList.contains("cancelar-inscricao-btn")) {
        const id = e.target.dataset.id;

        const confirmacao = await Swal.fire({
            title: "Cancelar inscrição?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sim, cancelar",
            cancelButtonText: "Não"
        });

        if (!confirmacao.isConfirmed) return;

        const token = getCookieToken();

        try {
            const response = await fetch(`/api/atividades/${id}/cancelar-inscricao`, {
                method: "POST",
                headers: { "Authorization": `Bearer ${token}` }
            });

            if (response.ok) {
                await Swal.fire({
                    icon: "success",
                    title: "Inscrição cancelada!",
                    timer: 2000,
                    showConfirmButton: false
                });
                await carregarAtividades();
            } else {
                const msg = await response.text();
                Swal.fire("Erro", msg, "error");
            }
        } catch (err) {
            console.error(err);
            Swal.fire("Erro", "Erro ao comunicar com o servidor", "error");
        }
    }

    if (e.target.id === "verInscritosBtn") {
        await carregarInscritos();
    }
});

async function carregarInscritos() {
    const lista = document.getElementById("listaInscritos");
    
    if (lista.style.display === "block") {
        lista.style.display = "none";
        return;
    }

    const token = getCookieToken();
    const idevento = parseInt(document.getElementById("ideventoHidden").value);

    try {
        const response = await fetch(`/api/eventos/${idevento}/inscritos`, {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) throw new Error("Erro ao buscar inscritos");

        const inscritos = await response.json();

        if (inscritos.length === 0) {
            lista.innerHTML = "<p class='text-muted mt-3'>Sem inscritos neste evento.</p>";
        } else {
            let html = "<ul class='list-group mt-3'>";
            inscritos.forEach(i => {
                html += `<li class='list-group-item'>${i.nome} (${i.email})</li>`;
            });
            html += "</ul>";
            lista.innerHTML = html;
        }

        lista.style.display = "block";
    } catch (err) {
        console.error("Erro ao buscar inscritos:", err);
        Swal.fire("Erro", "Não foi possível carregar a lista de inscritos.", "error");
    }
}
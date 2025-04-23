document.addEventListener("DOMContentLoaded", carregarAtividades);

async function carregarAtividades() {
    const token = localStorage.getItem("jwtToken");
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

            const botao = atividade.inscrito
                ? ""
                : `<button class="btn btn-outline-success inscrever-btn" data-id="${atividade.idatividade}">Inscrever-me</button>`;

            card.innerHTML = `
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <h5 class="card-title mb-1">${atividade.nome}</h5>
                        <p class="mb-1"><strong>Data:</strong> ${atividade.data} <strong>Hora:</strong> ${atividade.hora}</p>
                        <p class="mb-1"><strong>Quantidade Máxima:</strong> ${atividade.quantidademaxima}</p>
                    </div>
                    ${botao}
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
    const token = localStorage.getItem("jwtToken");
    const idevento = parseInt(document.getElementById("ideventoHidden").value);

    const atividade = {
        nome: document.getElementById("nomeAtividade").value,
        quantidademaxima: parseInt(document.getElementById("quantidadeMaxima").value),
        data: document.getElementById("dataAtividade").value,
        hora: document.getElementById("horaAtividade").value,
        idevento: idevento
    };

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
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalNovaAtividade'));
            modal.hide();

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

        const token = localStorage.getItem("jwtToken");

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
            } else {
                const msg = await response.text();
                Swal.fire("Erro", msg, "error");
            }
        } catch (err) {
            console.error(err);
            Swal.fire("Erro", "Erro ao comunicar com o servidor", "error");
        }
    }
});

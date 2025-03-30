document.getElementById("eventForm").addEventListener("submit", async function (event) {
    event.preventDefault();

    const nome = document.getElementById("eventName").value.trim();
    const descricao = document.getElementById("eventDescription").value.trim();
    const data = document.getElementById("eventDate").value;
    const hora = document.getElementById("eventTime").value;
    const local = document.getElementById("eventLocation").value.trim();
    const capacidade = parseInt(document.getElementById("eventCapacity").value);
    const idCategoria = parseInt(document.getElementById("eventCategory").value);
    const alertBox = document.getElementById("eventAlert");

    alertBox.classList.add("d-none");
    alertBox.textContent = "";

    // Validação
    if (!nome || !descricao || !data || !hora || !local || !capacidade || !idCategoria) {
        alertBox.textContent = "Preencha todos os campos obrigatórios.";
        alertBox.classList.remove("d-none");
        return;
    }

    if (isNaN(capacidade) || capacidade <= 0) {
        alertBox.textContent = "Capacidade inválida.";
        alertBox.classList.remove("d-none");
        return;
    }

    const eventoData = {
        nome,
        descricao,
        data,
        hora,
        local,
        capacidade,
        idCategoria: idCategoria
    };

    try {
        const response = await fetch("/api/eventos/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(eventoData)
        });

        if (response.ok) {
            alert("Evento registado com sucesso!");
            document.getElementById("eventForm").reset();
            $('#newEventModal').modal('hide');

            // Atualiza lista de eventos se houver função
            if (typeof loadEventos === "function") {
                loadEventos();
            } else {
                location.reload(); // alternativa
            }
        } else {
            const msg = await response.text();
            alertBox.textContent = msg || "Erro ao registar o evento.";
            alertBox.classList.remove("d-none");
        }
    } catch (err) {
        console.error("Erro:", err);
        alertBox.textContent = "Erro de ligação ao servidor.";
        alertBox.classList.remove("d-none");
    }
});

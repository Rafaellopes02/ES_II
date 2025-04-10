document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("eventForm");
    if (!form) return;

    form.addEventListener("submit", async function (event) {
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

        const token = localStorage.getItem("jwtToken");
        if (!token) {
            alert("Sessão expirada. Por favor, volte a iniciar sessão.");
            window.location.href = "/login";
            return;
        }

        const eventoData = {
            nome,
            descricao,
            data,
            hora,
            local,
            capacidade,
            idCategoria
        };

        try {
            const response = await fetch("/api/eventos/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(eventoData)
            });

            if (response.ok) {
                alert("Evento registado com sucesso!");
                form.reset();
                $('#newEventModal').modal('hide');

                if (typeof loadEventos === "function") {
                    loadEventos();
                } else {
                    location.reload();
                }
            } else if (response.status === 401) {
                alertBox.textContent = "Não autorizado. Faça login novamente.";
                alertBox.classList.remove("d-none");
                localStorage.removeItem("jwtToken");
                window.location.href = "/login";
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
});

document.addEventListener("DOMContentLoaded", function () {
    const editForm = document.getElementById("editEventForm");

    if (editForm) {
        editForm.addEventListener("submit", async function (event) {
            event.preventDefault();

            const id = document.getElementById("editEventId").value;
            const nome = document.getElementById("editEventName").value.trim();
            const descricao = document.getElementById("editEventDescription").value.trim();
            const data = document.getElementById("editEventDate").value;
            const hora = document.getElementById("editEventTime").value;
            const local = document.getElementById("editEventLocation").value.trim();
            const capacidade = parseInt(document.getElementById("editEventCapacity").value);
            const idCategoria = parseInt(document.getElementById("editEventCategory").value);
            const alertBox = document.getElementById("editEventAlert");

            alertBox.classList.add("d-none");
            alertBox.textContent = "";

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
                const response = await fetch(`/api/eventos/${id}`, {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`
                    },
                    body: JSON.stringify(eventoData)
                });

                if (response.ok) {
                    alert("Evento atualizado com sucesso!");
                    editForm.reset();
                    $('#EditEventModal').modal('hide');

                    if (typeof loadEventos === "function") {
                        loadEventos();
                    } else {
                        location.reload();
                    }
                } else {
                    const msg = await response.text();
                    alertBox.textContent = msg || "Erro ao atualizar o evento.";
                    alertBox.classList.remove("d-none");
                }
            } catch (err) {
                console.error("Erro:", err);
                alertBox.textContent = "Erro de ligação ao servidor.";
                alertBox.classList.remove("d-none");
            }
        });
    }

    // Preencher e abrir modal ao clicar no botão "Editar"
    document.body.addEventListener("click", function (e) {
        if (e.target.classList.contains("editar-btn")) {
            const btn = e.target;

            document.getElementById("editEventId").value = btn.dataset.id;
            document.getElementById("editEventName").value = btn.dataset.nome;
            document.getElementById("editEventDescription").value = btn.dataset.descricao;
            document.getElementById("editEventDate").value = btn.dataset.data.split("T")[0];
            document.getElementById("editEventTime").value = btn.dataset.hora;
            document.getElementById("editEventLocation").value = btn.dataset.local;
            document.getElementById("editEventCapacity").value = btn.dataset.capacidade;
            document.getElementById("editEventCategory").value = btn.dataset.categoria;

            document.getElementById("EditEventModalLabel").textContent = "Editar Evento";
            document.getElementById("editEventSubmitButton").textContent = "Guardar Alterações";

            $('#EditEventModal').modal('show');
        }
    });
    document.body.addEventListener("click", async (e) => {
        if (e.target.classList.contains("eliminar-btn")) {
            const id = e.target.dataset.id;

            if (!confirm("Tens a certeza que queres eliminar este evento?")) return;

            const token = localStorage.getItem("jwtToken");
            if (!token) {
                alert("Sessão expirada. Por favor, volte a iniciar sessão.");
                window.location.href = "/login";
                return;
            }

            try {
                const response = await fetch(`/api/eventos/${id}`, {
                    method: "DELETE",
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                });

                if (response.ok) {
                    alert("Evento eliminado com sucesso!");
                    if (typeof loadEventos === "function") {
                        loadEventos();
                    } else {
                        location.reload();
                    }
                } else {
                    const msg = await response.text();
                    alert(msg || "Erro ao eliminar o evento.");
                }
            } catch (error) {
                console.error("Erro ao eliminar evento:", error);
                alert("Erro de ligação ao servidor.");
            }
        }
    });
});

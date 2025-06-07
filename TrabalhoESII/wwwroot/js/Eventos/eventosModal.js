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
                await Swal.fire({
                    toast: true,
                    position: 'top-end', // canto superior direito
                    timerProgressBar: true,
                    icon: 'warning',
                    title: 'Sessão Expirada',
                    text: 'Por favor, volte a iniciar sessão.',
                    timer: 2000,
                    showConfirmButton: false
                });
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
                idCategoria,
                
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
                    await Swal.fire({
                        toast: true,
                        position: 'top-end', // canto superior direito
                        timerProgressBar: true,
                        icon: 'success',
                        title: 'Evento Atualizado!',
                        text: 'O evento foi atualizado com sucesso.',
                        timer: 2500,
                        showConfirmButton: false
                    });

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
        document.querySelectorAll('#EditEventModal .close, #EditEventModal .btn-secondary')
            .forEach(el => {
                el.addEventListener("click", () => {
                    $('#EditEventModal').modal('hide');
                });
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

            const confirmacao = await Swal.fire({
                title: "Tens a certeza?",
                text: "Esta ação não pode ser desfeita.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#3085d6",
                confirmButtonText: "Sim, eliminar",
                cancelButtonText: "Cancelar"
            });

            if (!confirmacao.isConfirmed) return;

            const token = localStorage.getItem("jwtToken");
            if (!token) {
                await Swal.fire({
                    toast: true,
                    position: 'top-end', // canto superior direito
                    timerProgressBar: true,
                    icon: 'warning',
                    title: 'Sessão Expirada',
                    text: 'Por favor, volte a iniciar sessão.',
                    timer: 2000,
                    showConfirmButton: false
                });
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
                    await Swal.fire({
                        icon: 'success',
                        toast: true,
                        position: 'top-end', // canto superior direito
                        timerProgressBar: true,
                        title: 'Eliminado',
                        text: 'O evento foi eliminado com sucesso.',
                        timer: 2500,
                        showConfirmButton: false
                    });

                    if (typeof loadEventos === "function") {
                        loadEventos();
                    } else {
                        location.reload();
                    }
                } else {
                    const msg = await response.text();
                    await Swal.fire({
                        toast: true,
                        position: 'top-end', // canto superior direito
                        timerProgressBar: true,
                        icon: 'error',
                        title: 'Erro ao eliminar',
                        text: msg || "Erro ao eliminar o evento.",
                        timer: 2000,
                        showConfirmButton: false
                    });
                }
            } catch (error) {
                console.error("Erro ao eliminar evento:", error);
                await Swal.fire({
                    toast: true,
                    position: 'top-end', // canto superior direito
                    timerProgressBar: true,
                    icon: 'error',
                    title: 'Erro de Ligação',
                    text: 'Erro de ligação ao servidor.',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("eventForm");

    if (form) {
        form.addEventListener("submit", async function (e) {
            e.preventDefault();

            const nome = document.getElementById("eventName").value.trim();
            const descricao = document.getElementById("eventDescription").value.trim();
            const data = document.getElementById("eventDate").value;
            const hora = document.getElementById("eventTime").value;
            const local = document.getElementById("eventLocation").value.trim();
            const capacidade = parseInt(document.getElementById("eventCapacity").value);
            const idCategoria = parseInt(document.getElementById("eventCategory").value);

            if (!nome || !descricao || !data || !hora || !local || isNaN(capacidade) || isNaN(idCategoria)) {
                await Swal.fire({
                    toast: true,
                    position: 'top-end',
                    timerProgressBar: true,
                    icon: 'warning',
                    title: 'Campos obrigatórios',
                    text: 'Preenche todos os campos antes de continuar.',
                    timer: 2500,
                    showConfirmButton: false
                });
                return;
            }

            const token = localStorage.getItem("jwtToken");
            if (!token) {
                await Swal.fire({
                    toast: true,
                    position: 'top-end',
                    timerProgressBar: true,
                    icon: 'warning',
                    title: 'Sessão Expirada',
                    text: 'Por favor, inicie sessão novamente.',
                    timer: 2000,
                    showConfirmButton: false
                });
                window.location.href = "/login";
                return;
            }

            const eventoData = {
                nome, descricao, data, hora, local, capacidade, idCategoria
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
                    const eventoCriado = await response.json();
                    const idevento = eventoCriado.idevento;

                    // Recolher os ingressos criados dinamicamente
                    const ingressos = [];
                    const tickets = document.querySelectorAll('#ticketsContainer > div');

                    tickets.forEach(ticket => {
                        const nome = ticket.querySelector(`[name="ticketName[]"]`).value;
                        const tipo = parseInt(ticket.querySelector(`[name="ticketType[]"]`).value);
                        const quantidade = parseInt(ticket.querySelector(`[name="ticketQuantity[]"]`).value);
                        const preco = parseFloat(ticket.querySelector(`[name="ticketPrice[]"]`).value);

                        ingressos.push({
                            nomeingresso: nome,
                            idtipoingresso: tipo,
                            quantidadedefinida: quantidade,
                            quantidadeatual: quantidade,
                            preco: preco,
                            idevento: idevento
                        });
                    });

                    // Submeter os ingressos, se existirem
                    if (ingressos.length > 0) {
                        const ingressosResponse = await fetch("/api/ingressos", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json",
                                "Authorization": `Bearer ${token}`
                            },
                            body: JSON.stringify(ingressos)
                        });

                        if (!ingressosResponse.ok) {
                            const msg = await ingressosResponse.text();
                            return await Swal.fire({
                                toast: true,
                                position: 'top-end',
                                timerProgressBar: true,
                                icon: 'error',
                                title: 'Erro ao registar ingressos',
                                text: msg || "O evento foi criado, mas houve erro ao criar os ingressos.",
                                timer: 3000,
                                showConfirmButton: false
                            });
                        }
                    }

                    await Swal.fire({
                        toast: true,
                        position: 'top-end',
                        timerProgressBar: true,
                        icon: 'success',
                        title: 'Evento Criado!',
                        text: 'O evento e os ingressos foram registados com sucesso.',
                        timer: 2500,
                        showConfirmButton: false
                    });

                    form.reset();
                    document.getElementById("ticketsContainer").innerHTML = ""; // limpa os ingressos
                    $('#newEventModal').modal('hide');

                    if (typeof loadEventos === "function") {
                        loadEventos();
                    } else {
                        location.reload();
                    }
                } else {
                    const msg = await response.text();
                    await Swal.fire({
                        toast: true,
                        position: 'top-end',
                        timerProgressBar: true,
                        icon: 'error',
                        title: 'Erro ao Registar',
                        text: msg || "Erro ao criar o evento.",
                        timer: 2000,
                        showConfirmButton: false
                    });
                }
            } catch (error) {
                console.error("Erro:", error);
                await Swal.fire({
                    toast: true,
                    position: 'top-end',
                    timerProgressBar: true,
                    icon: 'error',
                    title: 'Erro de Ligação',
                    text: 'Erro ao comunicar com o servidor.',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        });
    }
});

document.addEventListener("DOMContentLoaded", async function () {
    // Elementos da UI
    const loadingEl = document.getElementById("loading");
    const errorMessageEl = document.getElementById("errorMessage");
    const perfilInfoEl = document.getElementById("perfilInfo");
    const btnEditarEl = document.getElementById("btnEditar");
    const btnSalvarEl = document.getElementById("btnSalvar");
    const btnCancelarEl = document.getElementById("btnCancelar");
    const btnVoltarEl = document.getElementById("btnVoltar");
    const msgSucessoEl = document.getElementById("msgSucesso");

    // Elementos de visualização
    const viewElements = {
        nome: document.getElementById("viewNome"),
        email: document.getElementById("viewEmail"),
        idade: document.getElementById("viewIdade"),
        nacionalidade: document.getElementById("viewNacionalidade"),
        nomeUtilizador: document.getElementById("viewNomeUtilizador"),
        telefone: document.getElementById("viewTelefone"),
        tipoConta: document.getElementById("viewTipoConta")
    };

    // Elementos de edição
    const editElements = {
        nome: document.getElementById("editNome"),
        email: document.getElementById("editEmail"),
        idade: document.getElementById("editIdade"),
        nacionalidade: document.getElementById("editNacionalidade"),
        nomeUtilizador: document.getElementById("editNomeUtilizador"),
        telefone: document.getElementById("editTelefone"),
        senha: document.getElementById("editSenha"),
        confirmarSenha: document.getElementById("editConfirmarSenha")
    };

    // Dados originais do perfil
    let perfilData = null;

    // Verificar autenticação
    const token = localStorage.getItem("jwtToken");
    if (!token) {
        window.location.href = "/login";
        return;
    }

    // Buscar dados do perfil
    try {
        const response = await fetch("/api/perfil", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error(`Erro ao buscar perfil: ${response.status} ${await response.text()}`);
        }

        perfilData = await response.json();

        // Preencher dados de visualização
        viewElements.nome.textContent = perfilData.nome || "Não informado";
        viewElements.email.textContent = perfilData.email || "Não informado";
        viewElements.idade.textContent = perfilData.idade || "Não informado";
        viewElements.nacionalidade.textContent = perfilData.nacionalidade || "Não informado";
        viewElements.nomeUtilizador.textContent = perfilData.nomeUtilizador || "Não informado";
        viewElements.telefone.textContent = perfilData.telefone || "Não informado";

        // Definir tipo de conta
        let tipoContaText = "Utilizador padrão";
        if (perfilData.idTipoUtilizador === 1) {
            tipoContaText = "Administrador";
        } else if (perfilData.idTipoUtilizador === 2) {
            tipoContaText = "Utilizador registado";
        }
        viewElements.tipoConta.textContent = tipoContaText;

        // Preencher campos de edição
        editElements.nome.value = perfilData.nome || "";
        editElements.email.value = perfilData.email || "";
        editElements.idade.value = perfilData.idade || "";
        editElements.nacionalidade.value = perfilData.nacionalidade || "";
        editElements.nomeUtilizador.value = perfilData.nomeUtilizador || "";
        editElements.telefone.value = perfilData.telefone || "";

        // Mostrar UI
        loadingEl.style.display = "none";
        perfilInfoEl.style.display = "block";
        btnEditarEl.style.display = "block";

    } catch (error) {
        console.error(error);
        loadingEl.style.display = "none";
        errorMessageEl.style.display = "block";
        errorMessageEl.textContent = error.message || "Ocorreu um erro ao carregar os dados do perfil.";
    }

    // Evento de clique no botão Editar
    btnEditarEl.addEventListener("click", function() {
        // Mostrar modo de edição
        document.querySelectorAll(".view-mode").forEach(el => el.style.display = "none");
        document.querySelectorAll(".edit-mode").forEach(el => el.style.display = "block");

        // Mostrar/esconder botões relevantes
        btnEditarEl.style.display = "none";
        btnSalvarEl.style.display = "block";
        btnCancelarEl.style.display = "block";
    });

    // Evento de clique no botão Cancelar
    btnCancelarEl.addEventListener("click", function() {
        // Voltar para modo de visualização
        document.querySelectorAll(".view-mode").forEach(el => el.style.display = "block");
        document.querySelectorAll(".edit-mode").forEach(el => el.style.display = "none");

        // Restaurar valores originais nos campos de edição
        editElements.nome.value = perfilData.nome || "";
        editElements.email.value = perfilData.email || "";
        editElements.idade.value = perfilData.idade || "";
        editElements.nacionalidade.value = perfilData.nacionalidade || "";
        editElements.nomeUtilizador.value = perfilData.nomeUtilizador || "";
        editElements.telefone.value = perfilData.telefone || "";
        editElements.senha.value = "";
        editElements.confirmarSenha.value = "";

        // Esconder feedback de validação
        document.querySelectorAll(".invalid-feedback").forEach(el => el.style.display = "none");
        document.querySelectorAll(".form-control").forEach(el => el.classList.remove("is-invalid"));

        // Mostrar/esconder botões relevantes
        btnSalvarEl.style.display = "none";
        btnCancelarEl.style.display = "none";
        btnEditarEl.style.display = "block";
        msgSucessoEl.style.display = "none";
    });

    // Evento de clique no botão Salvar
    btnSalvarEl.addEventListener("click", async function() {
        // Validação
        let isValid = true;

        // Validar nome
        if (!editElements.nome.value.trim()) {
            editElements.nome.classList.add("is-invalid");
            isValid = false;
        } else {
            editElements.nome.classList.remove("is-invalid");
        }

        // Validar email
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        const emailValue = editElements.email?.value?.trim() || "";

        if (!emailValue || !emailRegex.test(emailValue)) {
            editElements.email.classList.add("is-invalid");
            isValid = false;
        } else {
            editElements.email.classList.remove("is-invalid");
        }

        // Validar nome de utilizador
        if (!editElements.nomeUtilizador.value.trim()) {
            editElements.nomeUtilizador.classList.add("is-invalid");
            isValid = false;
        } else {
            editElements.nomeUtilizador.classList.remove("is-invalid");
        }

        // Validar senhas
        if (editElements.senha.value) {
            if (editElements.senha.value.length < 6) {
                editElements.senha.classList.add("is-invalid");
                isValid = false;
            } else {
                editElements.senha.classList.remove("is-invalid");
            }

            if (editElements.senha.value !== editElements.confirmarSenha.value) {
                editElements.confirmarSenha.classList.add("is-invalid");
                isValid = false;
            } else {
                editElements.confirmarSenha.classList.remove("is-invalid");
            }
        }

        if (!isValid) {
            return;
        }

        // Criar objeto com dados atualizados
        const dadosAtualizados = {
            id: perfilData.id,
            nome: editElements.nome.value.trim(),
            email: editElements.email.value.trim(),
            idade: editElements.idade.value ? parseInt(editElements.idade.value) : null,
            telefone: editElements.telefone.value.trim(),
            nacionalidade: editElements.nacionalidade.value.trim(),
            nomeUtilizador: editElements.nomeUtilizador.value.trim(),
            senha: editElements.senha.value || null,
            idTipoUtilizador: perfilData.idTipoUtilizador
        };

        try {
            // Enviar dados para o servidor
            const response = await fetch("/api/perfil", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(dadosAtualizados)
            });

            if (!response.ok) {
                throw new Error(`Erro ao atualizar perfil: ${response.status} ${await response.text()}`);
            }

            // Atualizar objeto de dados
            perfilData = {
                ...perfilData,
                ...dadosAtualizados,
                senha: undefined
            };

            // Atualizar visualização
            viewElements.nome.textContent = perfilData.nome || "Não informado";
            viewElements.email.textContent = perfilData.email || "Não informado";
            viewElements.idade.textContent = perfilData.idade || "Não informado";
            viewElements.nacionalidade.textContent = perfilData.nacionalidade || "Não informado";
            viewElements.nomeUtilizador.textContent = perfilData.nomeUtilizador || "Não informado";
            viewElements.telefone.textContent = perfilData.telefone || "Não informado";

            // Voltar para modo de visualização
            document.querySelectorAll(".view-mode").forEach(el => el.style.display = "block");
            document.querySelectorAll(".edit-mode").forEach(el => el.style.display = "none");

            // Limpar campos de senha
            editElements.senha.value = "";
            editElements.confirmarSenha.value = "";

            // Mostrar mensagem de sucesso
            msgSucessoEl.style.display = "block";
            setTimeout(() => {
                msgSucessoEl.style.display = "none";
            }, 3000);

            // Mostrar/esconder botões relevantes
            btnSalvarEl.style.display = "none";
            btnCancelarEl.style.display = "none";
            btnEditarEl.style.display = "block";

        } catch (error) {
            console.error(error);
            errorMessageEl.style.display = "block";
            errorMessageEl.textContent = error.message || "Ocorreu um erro ao atualizar os dados do perfil.";
            window.scrollTo(0, 0);
        }
    });

    // Evento de clique no botão Voltar
    btnVoltarEl.addEventListener("click", function() {
        window.location.href = "/dashboard";
    });
});
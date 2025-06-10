
document.addEventListener("DOMContentLoaded", async () => {
    async function getUserInfo() {
        try {
            const response = await fetch("/api/auth/me", {
                credentials: "include"
            });

            if (!response.ok) throw new Error("Token inválido");

            const data = await response.json();
            return {
                userId: parseInt(data.userId),
                tipoUtilizador: parseInt(data.tipoUtilizador)
            };
        } catch (err) {
            console.error("Erro ao obter dados do utilizador:", err);
            return null;
        }
    }

    const userInfo = await getUserInfo();
    if (!userInfo || !userInfo.userId || !userInfo.tipoUtilizador) {
        window.location.href = "/login";
    }
});
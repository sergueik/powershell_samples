FROM ruanbekker/vscode-server:slim

ARG VERSION=2.2.0

ARG VSCODEUSER=coder
# NOTE: do not use
# ARG VSCODEUSER=$(whoami)
# it will be read literally
# NOTE: mkdir: cannot create directory ‘/home/$(whoami)’: Permission denied
ENV HOME=/home/${VSCODEUSER}

USER root

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /tmp

RUN curl -kLO https://github.com/zowe/zowe-explorer-vscode/releases/download/v${VERSION}/vscode-extension-for-zowe-${VERSION}.vsix \
    && mkdir -p ${HOME}/Files \
    && mv vscode-extension-for-zowe-${VERSION}.vsix ${HOME}/Files/zowe-explorer.vsix

# Create VS Code config directory properly
RUN mkdir -p "${HOME}/.local/share/code-server/User"

# Copy settings + keybindings to code server default user setting location

COPY Files/settings.json "${HOME}/.local/share/code-server/User/settings.json"
COPY Files/keybindings.json "${HOME}/.local/share/code-server/User/keybindings.json"
# NOTE: different directory layout than on VS Code
# COPY Files/settings.json $HOME/.config/Code/User/settings.json
# COPY Files/keybindings.json $HOME/.config/Code/User/keybindings.json

RUN chown -R ${VSCODEUSER}:${VSCODEUSER} ${HOME}

USER ${VSCODEUSER}

RUN code-server --install-extension ${HOME}/Files/zowe-explorer.vsix --force

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s \
CMD curl -f http://localhost:8080/ || exit 1


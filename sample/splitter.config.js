module.exports = {
    entry: "electron/Main.fsx",
    outDir: "out",
    babel: {
        presets: ["electron"],
        sourceMaps: true,
    },
    fable: {
        define: ["DEBUG"]
    }
}
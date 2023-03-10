import React, { useState } from "react";
import { PageLayout } from "./components/PageLayout";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import About from "./pages/About";
import NoPage from "./pages/NoPage";
import PageCaDocHome from "./pages/ca/Home"

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<PageLayout />}>
                    <Route index element={<Home />} />
                    <Route path="/ca" element={<PageCaDocHome />} />
                    <Route path="*" element={<NoPage />} />
                    <Route path="/about" element={<About />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;
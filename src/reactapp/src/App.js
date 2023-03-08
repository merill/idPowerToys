import React, { useState } from "react";
import { PageLayout } from "./components/PageLayout";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import About from "./pages/About";
import NoPage from "./pages/NoPage";
import PageCaDocHome from "./pages/cadoc/Home"

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<PageLayout />}>
                    <Route index element={<Home />} />
                    <Route path="/cadoc/Home" element={<PageCaDocHome />} />
                    <Route path="*" element={<NoPage />} />
                    <Route path="/about" element={<About />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;
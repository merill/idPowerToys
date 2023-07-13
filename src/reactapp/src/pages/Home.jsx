import React from 'react';
import { makeStyles, Title1, Subtitle1} from '@fluentui/react-components';
import { WhatsNewCards } from '../components/WhatsNewCards';

const useStyles = makeStyles({

});

function Home() {
    const styles = useStyles();

    return (
        <div className={styles.container}>
            <Title1 as="h1" block>Identity PowerToys</Title1>
            <p>idPowerToys is a collection of mini-apps for Microsoft Entra. We hope you find them useful.</p>
            <p>This app is a community project and is not an official Microsoft product or service.</p>
            <Subtitle1 as="h2" block>What's new</Subtitle1>
            <WhatsNewCards />
        </div>
    );
}

export default Home;
import React from 'react';
import { CaDocGenTabs } from '../../components/CaDocGenTabs';
import { makeStyles, typographyStyles } from '@fluentui/react-components';

const useStyles = makeStyles({
  text: typographyStyles.title2,
});

function Home() {
    const styles = useStyles();

    return (
        <>
            <span className={styles.text}>Conditional Access Documenter</span>
            <p>Export your conditional access policies as a PowerPoint presentation.</p>
            <CaDocGenTabs/>
        </>
    );
}

export default Home;
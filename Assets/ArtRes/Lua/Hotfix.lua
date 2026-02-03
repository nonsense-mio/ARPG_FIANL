xlua.hotfix(CS.HT.BossFightManager, {
    ActivateBossFight = function(self)
        if self.bossHasBeenDefeated then return end
        if self.bossFightIsActive then return end
        if self.boss == nil or not self.boss.isActiveAndEnabled then return end

        self.bossFightIsActive = true
        self.bossHasBeenAwakened = true

        if self.fogWalls ~= nil then
            self.fogWalls:ForEach(function(fog)
                if fog ~= nil then
                    fog:ActivateFogWall(true)
                end
            end)
        end
        --显示boss血条
        self:PublishBossHud()
        --更换背景音乐
        CS.MusicMgr.Instance:PlayBKMusic("Boss")
    end,
        OnCharacterDeath = function(self, character)
        -- 只在死亡对象是 Boss（EnemyManager）时触发 BossHasBeenDefeated
        if character == nil then return end
        -- enemyStatsManager 在 EnemyManager 上才存在，利用这一点判断类型并避免强转错误
        local stats = character.enemyStatsManager
        if stats == nil then return end
        if stats.isBoss then
            self:BossHasBeenDefeated()
            CS.MusicMgr.Instance:PlayBKMusic("Win")
        end
    end
})
